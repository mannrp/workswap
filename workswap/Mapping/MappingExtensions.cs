using workswap.DTOs;
using workswap.Models;

namespace workswap.Mapping;

/// <summary>
/// Extension methods for mapping domain entities to DTOs.
/// Centralizes mapping logic to maintain DRY principles and simplify services/controllers.
/// </summary>
public static class MappingExtensions
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

    public static DepartmentResponse ToResponse(this Department department)
    {
        return new DepartmentResponse(
            department.Id,
            department.Name,
            department.Description,
            department.Employees?.Count ?? 0,
            department.CreatedAt
        );
    }
}
