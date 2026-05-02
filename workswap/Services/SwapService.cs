using Microsoft.EntityFrameworkCore;
using workswap.Common;
using workswap.Data;
using workswap.DTOs;
using workswap.Mapping;
using workswap.Models;

namespace workswap.Services;

/// <summary>
/// Implementation of ISwapService managing shift swap logic and atomic transactions.
/// </summary>
public class SwapService : ISwapService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SwapService> _logger;

    public SwapService(ApplicationDbContext context, ILogger<SwapService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<SwapRequestResponse>>> GetMySwapsAsync(int userId)
    {
        var swaps = await _context.SwapRequests
            .Include(s => s.SenderShift)
                .ThenInclude(sh => sh.Department)
            .Include(s => s.Receiver)
            .Include(s => s.ReceiverShift)
                .ThenInclude(sh => sh.Department)
            .Where(s => s.SenderShift.AssignedUserId == userId || s.ReceiverId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<SwapRequestResponse>>.Success(
            swaps.Select(s => s.ToResponse())
        );
    }

    public async Task<Result<SwapRequestResponse>> CreateSwapAsync(int userId, CreateSwapDto dto)
    {
        var senderShift = await _context.Shifts.FindAsync(dto.SenderShiftId);
        if (senderShift == null)
            return Result<SwapRequestResponse>.NotFound("Your shift not found");

        if (senderShift.AssignedUserId != userId)
            return Result<SwapRequestResponse>.Forbidden("You do not own this shift");

        if (dto.ReceiverShiftId.HasValue)
        {
            var receiverShift = await _context.Shifts.FindAsync(dto.ReceiverShiftId.Value);
            if (receiverShift == null)
                return Result<SwapRequestResponse>.NotFound("Receiver shift not found");

            if (receiverShift.AssignedUserId != dto.ReceiverId)
                return Result<SwapRequestResponse>.Failure("Receiver does not own the specified shift");
        }

        var swap = new SwapRequest
        {
            SenderShiftId = dto.SenderShiftId,
            ReceiverId = dto.ReceiverId,
            ReceiverShiftId = dto.ReceiverShiftId,
            Status = SwapStatus.Pending
        };

        _context.SwapRequests.Add(swap);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Swap request {SwapId} created by user {UserId}", swap.Id, userId);

        // Reload to get navigation properties for the response
        var resultSwap = await _context.SwapRequests
            .Include(s => s.SenderShift)
            .Include(s => s.Receiver)
            .Include(s => s.ReceiverShift)
            .FirstAsync(s => s.Id == swap.Id);

        return Result<SwapRequestResponse>.Success(resultSwap.ToResponse());
    }

    public async Task<Result> RespondToSwapAsync(int swapId, int userId, bool accepted)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var swap = await _context.SwapRequests
                .Include(s => s.SenderShift)
                .Include(s => s.ReceiverShift)
                .FirstOrDefaultAsync(s => s.Id == swapId);

            if (swap == null)
                return Result.NotFound("Swap request not found");

            if (swap.ReceiverId != userId)
                return Result.Forbidden("You are not the receiver of this swap request");

            if (swap.Status != SwapStatus.Pending)
                return Result.Failure("This swap request is no longer pending");

            if (accepted)
            {
                swap.Status = SwapStatus.Completed;

                // Atomic shift transfer
                var senderShift = swap.SenderShift;
                var originalSenderId = senderShift.AssignedUserId;

                if (swap.ReceiverShiftId.HasValue)
                {
                    var receiverShift = swap.ReceiverShift;
                    // Trade: Swap owners
                    senderShift.AssignedUserId = swap.ReceiverId;
                    receiverShift.AssignedUserId = originalSenderId;
                }
                else
                {
                    // Direct Gift: Assign sender shift to receiver
                    senderShift.AssignedUserId = swap.ReceiverId;
                }

                _logger.LogInformation("Swap {SwapId} accepted. Shifts transferred.", swapId);
            }
            else
            {
                swap.Status = SwapStatus.Rejected;
                _logger.LogInformation("Swap {SwapId} rejected by receiver {UserId}", swapId, userId);
            }

            swap.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error processing swap response for swap {SwapId}", swapId);
            return Result.Failure("An error occurred while processing the swap request.");
        }
    }
}
