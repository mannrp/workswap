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

    public static NotificationResponse ToResponse(this Notification notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.Message,
            notification.IsRead,
            notification.CreatedAt,
            notification.ActionLink
        );
    }

    public static SwapRequestResponse ToResponse(this SwapRequest swap)
    {
        return new SwapRequestResponse(
            swap.Id,
            swap.SenderShiftId,
            swap.SenderShift?.ToResponse(),
            swap.ReceiverId,
            $"{swap.Receiver?.FirstName} {swap.Receiver?.LastName}",
            swap.ReceiverShiftId,
            swap.ReceiverShift?.ToResponse(),
            swap.Status,
            swap.CreatedAt,
            swap.UpdatedAt
        );
    }

    public static ShiftOfferResponse ToResponse(this ShiftOffer offer)
    {
        return new ShiftOfferResponse(
            offer.Id,
            offer.ShiftId,
            offer.Shift?.ToResponse(),
            offer.CreatedByUserId,
            $"{offer.CreatedByUser?.FirstName} {offer.CreatedByUser?.LastName}",
            offer.Status,
            offer.CreatedAt,
            offer.ExpiresAt
        );
    }
}
