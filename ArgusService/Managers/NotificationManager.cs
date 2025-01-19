// File: ArgusService/Managers/NotificationManager.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.Extensions.Logging;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Notification-related business logic.
    /// </summary>
    public class NotificationManager : INotificationManager
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationManager> _logger;

        public NotificationManager(INotificationRepository notificationRepository, ILogger<NotificationManager> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new notification.
        /// </summary>
        public async Task CreateNotificationAsync(Notification notification)
        {
            if (notification == null ||
                string.IsNullOrEmpty(notification.Type) ||
                string.IsNullOrEmpty(notification.Message) ||
                string.IsNullOrEmpty(notification.TrackerId))
            {
                throw new ArgumentException("Invalid notification data. Type, Message, and TrackerId are required.");
            }

            // Optionally validate notification.Type matches known types
            // e.g., if you only allow certain types like "motion" or "lockState"

            notification.Timestamp = DateTime.UtcNow;
            await _notificationRepository.AddNotificationAsync(notification);
            _logger.LogInformation("Notification '{NotificationType}' created for Tracker '{TrackerId}'.", notification.Type, notification.TrackerId);
        }

        /// <summary>
        /// Retrieves all notifications for a specific Tracker.
        /// </summary>
        public async Task<List<Notification>> GetNotificationsByTrackerIdAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.", nameof(trackerId));
            }

            _logger.LogInformation("Fetching notifications for Tracker '{TrackerId}'.", trackerId);

            var notifications = await _notificationRepository.GetNotificationsByTrackerIdAsync(trackerId);
            _logger.LogInformation("Fetched {Count} notifications for Tracker '{TrackerId}'.", notifications.Count, trackerId);

            return notifications;
        }
    }
}
