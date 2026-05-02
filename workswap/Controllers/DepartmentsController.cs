using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// CRUD operations for managing departments.
/// All endpoints require authentication.
/// </summary>
[Authorize]
public class DepartmentsController : ApiControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// Get all departments with employee counts.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAll()
    {
        var result = await _departmentService.GetAllAsync();
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific department by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponse>> GetById(int id)
    {
        var result = await _departmentService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new department.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DepartmentResponse>> Create(CreateDepartmentRequest request)
    {
        var result = await _departmentService.CreateAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Update an existing department.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentResponse>> Update(int id, UpdateDepartmentRequest request)
    {
        var result = await _departmentService.UpdateAsync(id, request);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a department. Employees will have their DepartmentId set to null.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _departmentService.DeleteAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all employees (users) belonging to a specific department.
    /// This is used to find colleagues for shift swaps.
    /// </summary>
    [HttpGet("{id}/employees")]
    public async Task<ActionResult<IEnumerable<dynamic>>> GetEmployees(int id)
    {
        var result = await _departmentService.GetEmployeesAsync(id);
        return HandleResult(result);
    }
}
