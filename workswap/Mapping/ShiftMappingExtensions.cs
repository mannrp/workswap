using workswap.DTOs;
using workswap.Models;

namespace workswap.Mapping;

/// <summary>
/// Extension methods for mapping Shift entities to DTOs.
/// Eliminates code duplication across controllers and services.
/// </summary>
public static class ShiftMappingExtensions
{
    public static ShiftResponse ToResponse(this Shift shift)
    {
        return new ShiftResponse(
            shift.Id,
            shift.Date,
            shift.StartTime,
            shift.EndTime,
            shift.Notes,
            shift.IsAvailableForSwap,
            shift.DepartmentId,
            shift.Department?.Name ?? "",
            shift.AssignedUserId,
            shift.AssignedUser != null
                ? $"{shift.AssignedUser.FirstName} {shift.AssignedUser.LastName}"
                : null,
            shift.CreatedAt
        );
    }
}
