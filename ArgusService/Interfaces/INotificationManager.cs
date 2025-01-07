using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface INotificationManager
    {
        Task CreateNotificationAsync(Notification notification);
        Task<List<Notification>> FetchNotificationsAsync(string userId);
    }
}
