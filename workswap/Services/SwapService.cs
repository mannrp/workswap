using Microsoft.EntityFrameworkCore;
using workswap.Data;
using workswap.DTOs;
using workswap.Models;
using workswap.Mapping;

namespace workswap.Services;

public class SwapService : ISwapService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SwapService> _logger;

    public SwapService(ApplicationDbContext context, ILogger<SwapService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SwapRequestResponse>> GetMySwapsAsync(int userId)
    {
        var swaps = await _context.SwapRequests
            .Include(sr => sr.SenderShift)
                .ThenInclude(ss => ss.Department)
            .Include(sr => sr.SenderShift.AssignedUser)
            .Include(sr => sr.ReceiverShift)
                .ThenInclude(rs => rs!.Department)
            .Include(sr => sr.ReceiverShift.AssignedUser)
            .Include(sr => sr.Sender)
            .Include(sr => sr.Receiver)
            .Where(sr => sr.SenderId == userId || sr.ReceiverId == userId)
            .ToListAsync();

        return swaps.Select(sr => new SwapRequestResponse(
            sr.Id,
            sr.SenderShiftId,
            sr.SenderShift.ToResponse(),
            sr.ReceiverShiftId,
            sr.ReceiverShift?.ToResponse(),
            sr.SenderId,
            $"{sr.Sender.FirstName} {sr.Sender.LastName}",
            sr.ReceiverId,
            $"{sr.Receiver.FirstName} {sr.Receiver.LastName}",
            sr.CreatedAt,
            sr.Status
        ));
    }

    public async Task<SwapRequestResponse> CreateSwapAsync(int userId, CreateSwapDto dto)
    {
        var senderShift = await _context.Shifts
            .Include(s => s.Department)
            .Include(s => s.AssignedUser)
            .FirstOrDefaultAsync(s => s.Id == dto.SenderShiftId);
        
        if (senderShift == null || senderShift.AssignedUserId != userId)
            throw new ArgumentException("Invalid sender shift");

        Shift? receiverShift = null;
        if (dto.ReceiverShiftId.HasValue)
        {
            receiverShift = await _context.Shifts
                .Include(s => s.Department)
                .Include(s => s.AssignedUser)
                .FirstOrDefaultAsync(s => s.Id == dto.ReceiverShiftId.Value);
            
            if (receiverShift == null)
                throw new ArgumentException("Invalid receiver shift");
        }

        var sender = await _context.Users.FindAsync(userId);
        var receiver = await _context.Users.FindAsync(dto.ReceiverId);
        
        if (sender == null || receiver == null)
            throw new ArgumentException("Invalid users");

        var swap = new SwapRequest
        {
            SenderShiftId = dto.SenderShiftId,
            ReceiverShiftId = dto.ReceiverShiftId,
            SenderId = userId,
            ReceiverId = dto.ReceiverId,
            Status = "Pending"
        };

        _context.SwapRequests.Add(swap);

        // Create notification for receiver
        var notification = new Notification
        {
            UserId = dto.ReceiverId,
            Message = $"{sender.FirstName} {sender.LastName} wants to swap shifts with you",
            ActionLink = $"swaps/{swap.Id}"
        };
        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Swap request {SwapId} created by user {UserId}", swap.Id, userId);

        return new SwapRequestResponse(
            swap.Id,
            swap.SenderShiftId,
            senderShift.ToResponse(),
            swap.ReceiverShiftId,
            receiverShift?.ToResponse(),
            swap.SenderId,
            $"{sender.FirstName} {sender.LastName}",
            swap.ReceiverId,
            $"{receiver.FirstName} {receiver.LastName}",
            swap.CreatedAt,
            swap.Status
        );
    }

    public async Task<bool> RespondToSwapAsync(int swapId, int userId, bool accepted)
    {
        var swap = await _context.SwapRequests
            .Include(sr => sr.SenderShift)
            .Include(sr => sr.ReceiverShift)
            .Include(sr => sr.Sender)
            .Include(sr => sr.Receiver)
            .FirstOrDefaultAsync(sr => sr.Id == swapId);

        if (swap == null)
            throw new ArgumentException("Swap request not found");

        if (swap.ReceiverId != userId)
            throw new UnauthorizedAccessException("You are not the receiver of this swap");

        if (swap.Status != "Pending")
            throw new InvalidOperationException("Swap is not pending");

        if (accepted)
        {
            // Validate ownership hasn't changed since request
            if (swap.SenderShift.AssignedUserId != swap.SenderId)
                throw new InvalidOperationException("Sender no longer owns the sender shift");

            if (swap.ReceiverShift != null && swap.ReceiverShift.AssignedUserId != swap.ReceiverId)
                throw new InvalidOperationException("Receiver no longer owns the receiver shift");

            // Perform the swap transactionally
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var tempUserId = swap.SenderShift.AssignedUserId;
                swap.SenderShift.AssignedUserId = swap.ReceiverShift?.AssignedUserId ?? swap.ReceiverId;
                if (swap.ReceiverShift != null)
                {
                    swap.ReceiverShift.AssignedUserId = tempUserId;
                }

                swap.Status = "Completed";

                // Create notifications
                var notificationForSender = new Notification
                {
                    UserId = swap.SenderId,
                    Message = "Your swap request has been accepted",
                    ActionLink = $"swaps/{swap.Id}"
                };

                var notificationForReceiver = new Notification
                {
                    UserId = userId,
                    Message = "You have accepted the swap request",
                    ActionLink = $"swaps/{swap.Id}"
                };

                _context.Notifications.AddRange(notificationForSender, notificationForReceiver);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                _logger.LogInformation("Swap request {SwapId} accepted by user {UserId}", swapId, userId);

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // Rejection path
        swap.Status = "Rejected";

        // Create notifications
        var notificationForSenderRejected = new Notification
        {
            UserId = swap.SenderId,
            Message = "Your swap request has been rejected",
            ActionLink = $"swaps/{swap.Id}"
        };

        var notificationForReceiverRejected = new Notification
        {
            UserId = userId,
            Message = "You have rejected the swap request",
            ActionLink = $"swaps/{swap.Id}"
        };

        _context.Notifications.AddRange(notificationForSenderRejected, notificationForReceiverRejected);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Swap request {SwapId} rejected by user {UserId}", swapId, userId);

        return true;
    }
}
