using workswap.DTOs;

namespace workswap.Services;

public interface IShiftOfferService
{
    Task<IEnumerable<ShiftOfferResponse>> GetActiveOffersAsync(int? departmentId = null);
    Task<ShiftOfferResponse> CreateOfferAsync(int shiftId, int userId, DateTime expiresAt);
    Task<ShiftOfferResponse?> ClaimOfferAsync(int offerId, int userId);
}
