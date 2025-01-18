using ArgusService.Interfaces;
using ArgusService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ArgusService.Data;

namespace ArgusService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(ApplicationDbContext context, ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Saves a new notification into the database.
        /// </summary>
        public async Task AddNotificationAsync(Notification notification)
        {
            if (notification == null)
            {
                _logger.LogWarning("Attempted to add a null notification.");
                throw new ArgumentException("Notification cannot be null.");
            }

            _logger.LogInformation("Adding new notification for UserId '{UserId}'.", notification.UserId);

            // Optionally verify user existence if needed
            // e.g., check if user exists in Users table

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification added successfully for UserId '{UserId}'.", notification.UserId);
        }

        /// <summary>
        /// Fetches all notifications for a specific user.
        /// </summary>
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            _logger.LogInformation("Fetching notifications for UserId '{UserId}'.", userId);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID is null or empty. Cannot fetch notifications.");
                throw new ArgumentException("User ID cannot be null or empty.");
            }

            var notifications = await _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Timestamp)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} notifications for UserId '{UserId}'.", notifications.Count, userId);
            return notifications;
        }
    }
}
