using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for managing shift offers on the open marketplace.
/// </summary>
public interface IShiftOfferService
{
    /// <summary>
    /// Retrieves all active shift offers, optionally filtered by department.
    /// </summary>
    Task<Result<IEnumerable<ShiftOfferResponse>>> GetActiveOffersAsync(int? departmentId = null);

    /// <summary>
    /// Creates a new shift offer.
    /// </summary>
    Task<Result<ShiftOfferResponse>> CreateOfferAsync(int shiftId, int userId, DateTime? expiresAt = null);

    /// <summary>
    /// Claims an existing shift offer.
    /// </summary>
    Task<Result<ShiftOfferResponse>> ClaimOfferAsync(int offerId, int userId);
}
