using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface INotification
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetNotificationsByUserIdAsync(string userId);
    }
}
