using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for managing shifts and their assignments.
/// </summary>
public interface IShiftService
{
    /// <summary>
    /// Retrieves all shifts matching the specified criteria.
    /// </summary>
    Task<Result<IEnumerable<ShiftResponse>>> GetAllAsync(
        int? departmentId = null,
        int? userId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        bool? availableForSwap = null);
    
    /// <summary>
    /// Retrieves a specific shift by ID.
    /// </summary>
    Task<Result<ShiftResponse>> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new shift.
    /// </summary>
    Task<Result<ShiftResponse>> CreateAsync(CreateShiftRequest request);

    /// <summary>
    /// Updates an existing shift.
    /// </summary>
    Task<Result<ShiftResponse>> UpdateAsync(int id, UpdateShiftRequest request);

    /// <summary>
    /// Deletes a shift.
    /// </summary>
    Task<Result> DeleteAsync(int id);
}
