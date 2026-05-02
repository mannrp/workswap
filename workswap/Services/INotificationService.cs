using workswap.Common;
using workswap.DTOs;

namespace workswap.Services;

/// <summary>
/// Service for managing user notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Retrieves all notifications for a specific user.
    /// </summary>
    Task<Result<IEnumerable<NotificationResponse>>> GetUserNotificationsAsync(int userId);

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    Task<Result> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// Marks all unread notifications for a user as read.
    /// </summary>
    Task<Result<int>> MarkAllAsReadAsync(int userId);
}
