using Microsoft.EntityFrameworkCore;
using workswap.Common;
using workswap.Data;
using workswap.DTOs;
using workswap.Mapping;

namespace workswap.Services;

/// <summary>
/// Implementation of INotificationService.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<NotificationResponse>>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<NotificationResponse>>.Success(
            notifications.Select(n => n.ToResponse())
        );
    }

    public async Task<Result> MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null)
        {
            return Result.NotFound("Notification not found");
        }

        if (notification.IsRead)
        {
            return Result.Success();
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", notificationId, userId);

        return Result.Success();
    }

    public async Task<Result<int>> MarkAllAsReadAsync(int userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (!unreadNotifications.Any())
        {
            return Result<int>.Success(0);
        }

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Marked {Count} notifications as read for user {UserId}", unreadNotifications.Count, userId);

        return Result<int>.Success(unreadNotifications.Count);
    }
}
