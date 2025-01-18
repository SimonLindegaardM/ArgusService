using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Notification repository operations.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Saves a new notification into the database.
        /// </summary>
        Task AddNotificationAsync(Notification notification);

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        Task<List<Notification>> GetNotificationsByUserIdAsync(string userId);
    }
}
