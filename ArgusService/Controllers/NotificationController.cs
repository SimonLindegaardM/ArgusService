using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationManager _notificationManager;

        public NotificationController(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// Creates a new notification for a user or device.
        /// </summary>
        [HttpPost("notifications")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid().ToString(),
                    Type = request.Type,
                    Timestamp = DateTime.UtcNow,
                    Message = request.Message,
                    UserId = request.UserId
                };

                await _notificationManager.CreateNotificationAsync(notification);
                return Ok(new { Message = "Notification created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            try
            {
                var notifications = await _notificationManager.FetchNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class CreateNotificationRequest
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
    }
}
