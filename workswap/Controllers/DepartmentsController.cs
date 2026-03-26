using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using workswap.Data;
using workswap.DTOs;
using workswap.Models;

namespace workswap.Controllers;

/// <summary>
/// CRUD operations for managing departments.
/// All endpoints require authentication.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all departments with employee counts.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAll()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .Select(d => new DepartmentResponse(
                d.Id,
                d.Name,
                d.Description,
                d.Employees.Count,
                d.CreatedAt
            ))
            .ToListAsync();

        return Ok(departments);
    }

    /// <summary>
    /// Get a specific department by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponse>> GetById(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .Where(d => d.Id == id)
            .Select(d => new DepartmentResponse(
                d.Id,
                d.Name,
                d.Description,
                d.Employees.Count,
                d.CreatedAt
            ))
            .FirstOrDefaultAsync();

        if (department == null)
        {
            return NotFound(new { message = $"Department with ID {id} not found." });
        }

        return Ok(department);
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create(CreateDepartmentRequest request)
    {
        // Check for duplicate name
        if (await _context.Departments.AnyAsync(d => d.Name == request.Name))
        {
            return BadRequest(new { message = "A department with this name already exists." });
        }

        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var response = new DepartmentResponse(
            department.Id,
            department.Name,
            department.Description,
            0, // New department has no employees
            department.CreatedAt
        );

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, response);
    }

    /// <summary>
    /// Update an existing department.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentResponse>> Update(int id, UpdateDepartmentRequest request)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return NotFound(new { message = $"Department with ID {id} not found." });
        }

        // Check for duplicate name (excluding current department)
        if (await _context.Departments.AnyAsync(d => d.Name == request.Name && d.Id != id))
        {
            return BadRequest(new { message = "A department with this name already exists." });
        }

        department.Name = request.Name;
        department.Description = request.Description;

        await _context.SaveChangesAsync();

        var response = new DepartmentResponse(
            department.Id,
            department.Name,
            department.Description,
            department.Employees.Count,
            department.CreatedAt
        );

        return Ok(response);
    }

    /// <summary>
    /// Delete a department. Employees will have their DepartmentId set to null.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return NotFound(new { message = $"Department with ID {id} not found." });
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get all employees (users) belonging to a specific department.
    /// This is used to find colleagues for shift swaps.
    /// </summary>
    [HttpGet("{id}/employees")]
    public async Task<ActionResult<IEnumerable<dynamic>>> GetEmployees(int id)
    {
        // First check if the department exists
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == id);
        if (!departmentExists)
        {
            return NotFound(new { message = "Department not found" });
        }

        // Fetch users assigned to this department
        // We only return simple info: Id, Name, and Email
        var employees = await _context.Users
            .Where(u => u.DepartmentId == id)
            .OrderBy(u => u.FirstName)
            .Select(u => new
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = u.FirstName + " " + u.LastName,
                Email = u.Email
            })
            .ToListAsync();

        return Ok(employees);
    }
}
