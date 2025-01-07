using ArgusService.Interfaces;
using ArgusService.Models;
using ArgusService.Repositories;

namespace ArgusService.Managers
{
    public class NotificationManager : INotificationManager
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationManager(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            if (notification == null || string.IsNullOrEmpty(notification.Type) || string.IsNullOrEmpty(notification.Message))
            {
                throw new ArgumentException("Invalid notification content.");
            }

            await _notificationRepository.AddNotificationAsync(notification);
        }

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
