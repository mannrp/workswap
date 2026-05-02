using Microsoft.EntityFrameworkCore;
using workswap.Common;
using workswap.Data;
using workswap.DTOs;
using workswap.Mapping;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// Implementation of IShiftOfferService for marketplace operations.
/// </summary>
public class ShiftOfferService : IShiftOfferService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShiftOfferService> _logger;

    public ShiftOfferService(ApplicationDbContext context, ILogger<ShiftOfferService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<ShiftOfferResponse>>> GetActiveOffersAsync(int? departmentId = null)
    {
        var query = _context.ShiftOffers
            .Include(o => o.Shift)
                .ThenInclude(s => s.Department)
            .Include(o => o.OfferedBy)
            .Include(o => o.ClaimedBy)
            .Where(o => o.Status == OfferStatus.Active && o.ExpiresAt > DateTime.UtcNow);

        if (departmentId.HasValue)
        {
            query = query.Where(o => o.Shift.DepartmentId == departmentId.Value);
        }

        var offers = await query.OrderBy(o => o.Shift.StartTime).ToListAsync();

        return Result<IEnumerable<ShiftOfferResponse>>.Success(
            offers.Select(o => o.ToResponse())
        );
    }

    public async Task<Result<ShiftOfferResponse>> CreateOfferAsync(int shiftId, int userId, DateTime? expiresAt = null)
    {
        var shift = await _context.Shifts.FindAsync(shiftId);
        if (shift == null)
            return Result<ShiftOfferResponse>.NotFound("Shift not found");

        if (shift.AssignedUserId != userId)
            return Result<ShiftOfferResponse>.Forbidden("You do not own this shift");

        // Check for existing active offer
        if (await _context.ShiftOffers.AnyAsync(o => o.ShiftId == shiftId && o.Status == OfferStatus.Active))
            return Result<ShiftOfferResponse>.Failure("This shift is already offered in the marketplace");

        var offer = new ShiftOffer
        {
            ShiftId = shiftId,
            OfferedById = userId,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7),
            Status = OfferStatus.Active
        };

        _context.ShiftOffers.Add(offer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift {ShiftId} offered by user {UserId}", shiftId, userId);

        // Reload for response
        var resultOffer = await _context.ShiftOffers
            .Include(o => o.Shift)
            .Include(o => o.OfferedBy)
            .FirstAsync(o => o.Id == offer.Id);

        return Result<ShiftOfferResponse>.Success(resultOffer.ToResponse());
    }

    public async Task<Result<ShiftOfferResponse>> ClaimOfferAsync(int offerId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var offer = await _context.ShiftOffers
                .Include(o => o.Shift)
                .Include(o => o.OfferedBy)
                .Include(o => o.ClaimedBy)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer == null)
                return Result<ShiftOfferResponse>.NotFound("Offer not found");

            if (offer.Status != OfferStatus.Active || offer.ExpiresAt <= DateTime.UtcNow)
                return Result<ShiftOfferResponse>.Failure("This offer is no longer active");

            if (offer.OfferedById == userId)
                return Result<ShiftOfferResponse>.Failure("You cannot claim your own offer");

            // Update offer status
            offer.Status = OfferStatus.Claimed;
            offer.ClaimedById = userId;

            // Transfer shift
            offer.Shift.AssignedUserId = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Offer {OfferId} claimed by user {UserId}. Shift {ShiftId} transferred.", offerId, userId, offer.ShiftId);

            return Result<ShiftOfferResponse>.Success(offer.ToResponse());
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error claiming offer {OfferId}", offerId);
            return Result<ShiftOfferResponse>.Failure("An error occurred while claiming the offer.");
        }
    }
}
