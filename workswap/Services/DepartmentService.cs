using Microsoft.EntityFrameworkCore;
using workswap.Common;
using workswap.Data;
using workswap.DTOs;
using workswap.Mapping;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// Implementation of IDepartmentService handling business logic for departments.
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(ApplicationDbContext context, ILogger<DepartmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<DepartmentResponse>>> GetAllAsync()
    {
        var departments = await _context.Departments
            .Include(d => d.Employees)
            .OrderBy(d => d.Name)
            .ToListAsync();

        return Result<IEnumerable<DepartmentResponse>>.Success(
            departments.Select(d => d.ToResponse())
        );
    }

    public async Task<Result<DepartmentResponse>> GetByIdAsync(int id)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return Result<DepartmentResponse>.NotFound($"Department with ID {id} not found.");
        }

        return Result<DepartmentResponse>.Success(department.ToResponse());
    }

    public async Task<Result<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request)
    {
        if (await _context.Departments.AnyAsync(d => d.Name == request.Name))
        {
            return Result<DepartmentResponse>.Failure("A department with this name already exists.");
        }

        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Department {DepartmentName} created with ID {DepartmentId}", department.Name, department.Id);

        return Result<DepartmentResponse>.Success(department.ToResponse());
    }

    public async Task<Result<DepartmentResponse>> UpdateAsync(int id, UpdateDepartmentRequest request)
    {
        var department = await _context.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
        {
            return Result<DepartmentResponse>.NotFound($"Department with ID {id} not found.");
        }

        if (await _context.Departments.AnyAsync(d => d.Name == request.Name && d.Id != id))
        {
            return Result<DepartmentResponse>.Failure("A department with this name already exists.");
        }

        department.Name = request.Name;
        department.Description = request.Description;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Department {DepartmentId} updated", id);

        return Result<DepartmentResponse>.Success(department.ToResponse());
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
        {
            return Result.NotFound($"Department with ID {id} not found.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Department {DepartmentId} deleted", id);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<dynamic>>> GetEmployeesAsync(int id)
    {
        if (!await _context.Departments.AnyAsync(d => d.Id == id))
        {
            return Result<IEnumerable<dynamic>>.NotFound("Department not found");
        }

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

        return Result<IEnumerable<dynamic>>.Success(employees);
    }
}
