// File: ArgusService/Controllers/NotificationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using ArgusService.Models;

namespace ArgusService.Controllers
{
    /// <summary>
    /// Controller for managing Notifications.
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IMapper _mapper;

        public NotificationsController(INotificationRepository notificationRepository, ILogger<NotificationsController> logger, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new notification.
        /// </summary>
        /// <param name="dto">The notification details.</param>
        /// <returns>Result of the creation operation.</returns>
        /// <response code="200">Notification added successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>List of notifications.</returns>
        /// <response code="200">Returns the list of notifications.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
