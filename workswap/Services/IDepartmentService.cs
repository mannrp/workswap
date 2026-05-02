using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for managing departments and their relationships.
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// Retrieves all departments with employee counts.
    /// </summary>
    Task<Result<IEnumerable<DepartmentResponse>>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific department by ID.
    /// </summary>
    Task<Result<DepartmentResponse>> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new department.
    /// </summary>
    Task<Result<DepartmentResponse>> CreateAsync(CreateDepartmentRequest request);

    /// <summary>
    /// Updates an existing department.
    /// </summary>
    Task<Result<DepartmentResponse>> UpdateAsync(int id, UpdateDepartmentRequest request);

    /// <summary>
    /// Deletes a department.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Gets all employees belonging to a specific department.
    /// </summary>
    Task<Result<IEnumerable<dynamic>>> GetEmployeesAsync(int id);
}
