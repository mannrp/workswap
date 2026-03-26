using Microsoft.EntityFrameworkCore;
using workswap.Data;
using workswap.DTOs;
using workswap.Models;
using workswap.Mapping;

namespace workswap.Services;

public class ShiftOfferService : IShiftOfferService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ShiftOfferService> _logger;

    public ShiftOfferService(ApplicationDbContext context, ILogger<ShiftOfferService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ShiftOfferResponse>> GetActiveOffersAsync(int? departmentId = null)
    {
        var query = _context.ShiftOffers
            .Include(so => so.Shift)
                .ThenInclude(s => s.Department)
            .Include(so => so.Shift.AssignedUser)
            .Include(so => so.OfferedBy)
            .Include(so => so.ClaimedBy)
            .Where(so => so.Status == "Active")
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(so => so.Shift.DepartmentId == departmentId.Value);
        }

        var offers = await query
            .OrderByDescending(so => so.CreatedAt)
            .ToListAsync();

        return offers.Select(so => new ShiftOfferResponse(
            so.Id,
            so.ShiftId,
            so.Shift.ToResponse(),
            so.OfferedById,
            $"{so.OfferedBy.FirstName} {so.OfferedBy.LastName}",
            so.ClaimedById,
            so.ClaimedBy != null ? $"{so.ClaimedBy.FirstName} {so.ClaimedBy.LastName}" : null,
            so.CreatedAt,
            so.ExpiresAt,
            so.Status
        ));
    }

    public async Task<ShiftOfferResponse> CreateOfferAsync(int shiftId, int userId, DateTime expiresAt)
    {
        var shift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == shiftId);

        if (shift == null)
            throw new ArgumentException("Shift not found");

        if (shift.AssignedUserId != userId)
            throw new UnauthorizedAccessException("You are not assigned to this shift");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var offer = new ShiftOffer
        {
            ShiftId = shiftId,
            OfferedById = userId,
            ExpiresAt = expiresAt,
            Status = "Active"
        };

        _context.ShiftOffers.Add(offer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Shift offer {OfferId} created for shift {ShiftId} by user {UserId}", offer.Id, shiftId, userId);

        return new ShiftOfferResponse(
            offer.Id,
            offer.ShiftId,
            shift.ToResponse(),
            offer.OfferedById,
            $"{user.FirstName} {user.LastName}",
            null,
            null,
            offer.CreatedAt,
            offer.ExpiresAt,
            offer.Status
        );
    }

    public async Task<ShiftOfferResponse?> ClaimOfferAsync(int offerId, int userId)
    {
        var offer = await _context.ShiftOffers
            .Include(so => so.Shift)
                .ThenInclude(s => s.Department)
            .Include(so => so.Shift.AssignedUser)
            .Include(so => so.OfferedBy)
            .FirstOrDefaultAsync(so => so.Id == offerId);

        if (offer == null)
            return null;

        if (offer.Status != "Active")
            throw new InvalidOperationException("Offer is not active");

        if (offer.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("Offer has expired");

        if (offer.OfferedById == userId)
            throw new InvalidOperationException("You cannot claim your own offer");

        var claimer = await _context.Users.FindAsync(userId);
        if (claimer == null)
            throw new ArgumentException("User not found");

        // Perform the claim transactionally
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            // Reassign the shift
            offer.Shift.AssignedUserId = userId;
            offer.ClaimedById = userId;
            offer.Status = "Claimed";

            // Create notifications
            var notificationForOfferer = new Notification
            {
                UserId = offer.OfferedById,
                Message = $"{claimer.FirstName} {claimer.LastName} has claimed your shift offer",
                ActionLink = $"offers/{offer.Id}"
            };

            var notificationForClaimer = new Notification
            {
                UserId = userId,
                Message = "You have successfully claimed a shift",
                ActionLink = $"offers/{offer.Id}"
            };

            _context.Notifications.AddRange(notificationForOfferer, notificationForClaimer);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            _logger.LogInformation("Shift offer {OfferId} claimed by user {UserId}", offerId, userId);

            return new ShiftOfferResponse(
                offer.Id,
                offer.ShiftId,
                offer.Shift.ToResponse(),
                offer.OfferedById,
                $"{offer.OfferedBy.FirstName} {offer.OfferedBy.LastName}",
                offer.ClaimedById,
                $"{claimer.FirstName} {claimer.LastName}",
                offer.CreatedAt,
                offer.ExpiresAt,
                offer.Status
            );
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
