// File: ArgusService/Controllers/NotificationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // INotificationRepository interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper; // Add this
using ArgusService.Models; // Ensure this is included for the Notification model

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IMapper _mapper; // Inject IMapper

        public NotificationsController(INotificationRepository notificationRepository, ILogger<NotificationsController> logger, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _mapper = mapper; // Assign IMapper
        }

        /// <summary>
        /// Creates a new notification.
        /// Example body:
        /// {
        ///   "trackerId": "Tracker001",
        ///   "type": "motion",
        ///   "message": "Motion detected on tracker.",
        ///   "timestamp": "2025-01-17T06:00:00Z"
        /// }
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> AddNotification([FromBody] NotificationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid NotificationRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Model
                var notification = _mapper.Map<Notification>(dto);

                await _notificationRepository.AddNotificationAsync(notification);
                _logger.LogInformation("Notification added for Tracker '{TrackerId}'.", dto.TrackerId);
                return Ok(new { Message = "Notification added successfully." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while adding notification for Tracker '{TrackerId}'.", dto.TrackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding notification for Tracker '{TrackerId}'.", dto.TrackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// Example GET: /api/notifications/{userId}
        /// </summary>
        [HttpGet("{userId}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetNotificationsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID is required to fetch notifications.");
                return BadRequest(new { Message = "User ID is required." });
            }

            try
            {
                // Assuming the correct method name is GetNotificationsByUserIdAsync
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                _logger.LogInformation("Fetched notifications for User '{UserId}'.", userId);
                return Ok(notifications);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while fetching notifications for User '{UserId}'.", userId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications for User '{UserId}'.", userId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
