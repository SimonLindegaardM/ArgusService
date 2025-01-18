using ArgusService.Interfaces;
using ArgusService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Notification-related business logic.
    /// </summary>
    public class NotificationManager : INotificationManager
    {
        private readonly INotificationRepository _notificationRepository;

        /// <summary>
        /// Initializes a new instance of NotificationManager.
        /// </summary>
        /// <param name="notificationRepository">The notification repository.</param>
        public NotificationManager(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Creates a new notification.
        /// </summary>
        public async Task CreateNotificationAsync(Notification notification)
        {
            if (notification == null ||
                string.IsNullOrEmpty(notification.Type) ||
                string.IsNullOrEmpty(notification.Message) ||
                string.IsNullOrEmpty(notification.UserId))
            {
                throw new ArgumentException("Invalid notification data. Type, Message, and UserId are required.");
            }

            // Optionally validate notification.Type matches known types
            // e.g., if you only allow certain types like "motion" or "lockState"

            notification.Timestamp = DateTime.UtcNow;
            await _notificationRepository.AddNotificationAsync(notification);
        }

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        public async Task<List<Notification>> FetchNotificationsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
        }
    }
}
