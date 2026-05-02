using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workswap.DTOs;
using workswap.Extensions;
using workswap.Services;

namespace workswap.Controllers;

/// <summary>
/// Endpoints for managing user notifications.
/// </summary>
[Authorize]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get all notifications for the current user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetNotifications()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.GetUserNotificationsAsync(userId);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark a specific notification as read.
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = User.GetUserId();
        var result = await _notificationService.MarkAsReadAsync(id, userId);
        return HandleResult(result);
    }

    /// <summary>
    /// Mark all unread notifications for the current user as read.
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.GetUserId();
        var result = await _notificationService.MarkAllAsReadAsync(userId);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = $"{result.Value} notifications marked as read" });
        }
        
        return HandleResult(result);
    }
}