﻿using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Notification manager operations.
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Creates a new notification.
        /// </summary>
        Task CreateNotificationAsync(Notification notification);

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        Task<List<Notification>> FetchNotificationsAsync(string userId);
    }
}
