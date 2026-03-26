namespace workswap.DTOs;

// ─────────────────────────────────────────────────────────────────────────────
// DEPARTMENT DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Request to create a new department.
/// </summary>
public record CreateDepartmentRequest(
    string Name,
    string? Description = null
);

/// <summary>
/// Request to update an existing department.
/// </summary>
public record UpdateDepartmentRequest(
    string Name,
    string? Description = null
);

/// <summary>
/// Response containing department data.
/// </summary>
public record DepartmentResponse(
    int Id,
    string Name,
    string? Description,
    int EmployeeCount,
    DateTime CreatedAt
);

// ─────────────────────────────────────────────────────────────────────────────
// SHIFT DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Request to create a new shift.
/// </summary>
public record CreateShiftRequest(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DepartmentId,
    int? AssignedUserId = null,
    string? Notes = null
);

/// <summary>
/// Request to update an existing shift.
/// </summary>
public record UpdateShiftRequest(
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int DepartmentId,
    int? AssignedUserId = null,
    string? Notes = null,
    bool IsAvailableForSwap = false
);

/// <summary>
/// Response containing shift data with related entities.
/// </summary>
public record ShiftResponse(
    int Id,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Notes,
    bool IsAvailableForSwap,
    int DepartmentId,
    string DepartmentName,
    int? AssignedUserId,
    string? AssignedUserName,
    DateTime CreatedAt
);

// ─────────────────────────────────────────────────────────────────────────────
// SHIFT OFFER DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Response containing shift offer data.
/// </summary>
public record ShiftOfferResponse(
    int Id,
    int ShiftId,
    ShiftResponse Shift,
    int OfferedById,
    string OfferedByName,
    int? ClaimedById,
    string? ClaimedByName,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    string Status
);

// ─────────────────────────────────────────────────────────────────────────────
// SWAP REQUEST DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Response containing swap request data.
/// </summary>
public record SwapRequestResponse(
    int Id,
    int SenderShiftId,
    ShiftResponse SenderShift,
    int? ReceiverShiftId,
    ShiftResponse? ReceiverShift,
    int SenderId,
    string SenderName,
    int ReceiverId,
    string ReceiverName,
    DateTime CreatedAt,
    string Status
);

// ─────────────────────────────────────────────────────────────────────────────
// NOTIFICATION DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Response containing notification data.
/// </summary>
public record NotificationResponse(
    int Id,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    string? ActionLink
);

// ─────────────────────────────────────────────────────────────────────────────
// SWAP REQUEST INPUT DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Request to create a swap between two shifts.
/// </summary>
public record CreateSwapDto(
    int SenderShiftId,
    int? ReceiverShiftId,
    int ReceiverId
);

/// <summary>
/// Response to a swap request (accept or reject).
/// </summary>
public record SwapResponseDto(
    bool Accepted
);

// ─────────────────────────────────────────────────────────────────────────────
// SHIFT OFFER INPUT DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Request to create a shift offer.
/// </summary>
public record CreateOfferDto(
    DateTime? ExpiresAt = null
);


