using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class NotificationRepository : INotification
    {
        private readonly MyDbContext _context;

        public NotificationRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a new notification into the database.
        /// </summary>
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();
        }
    }
}
