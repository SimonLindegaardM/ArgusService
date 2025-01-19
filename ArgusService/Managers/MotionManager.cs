// File: ArgusService/Managers/MotionManager.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.Extensions.Logging;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Motion-related business logic.
    /// </summary>
    public class MotionManager : IMotionManager
    {
        private readonly IMotionRepository _motionRepository;
        private readonly ITrackerRepository _trackerRepository; // Added
        private readonly INotificationManager _notificationManager; // Added
        private readonly ILogger<MotionManager> _logger;

        public MotionManager(
            IMotionRepository motionRepository,
            ITrackerRepository trackerRepository, // Added
            INotificationManager notificationManager, // Added
            ILogger<MotionManager> logger)
        {
            _motionRepository = motionRepository;
            _trackerRepository = trackerRepository; // Added
            _notificationManager = notificationManager; // Added
            _logger = logger;
        }

        /// <summary>
        /// Logs a motion detection event for a Tracker.
        /// </summary>
        public async Task LogMotionEventAsync(string trackerId, bool motionDetected, DateTime timestamp)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.", nameof(trackerId));

            var motionEvent = new Motion
            {
                TrackerId = trackerId,
                MotionDetected = motionDetected,
                Timestamp = timestamp
            };

            _logger.LogInformation("Logging motion event for Tracker '{TrackerId}': {MotionDetected} at {Timestamp}.", trackerId, motionDetected, timestamp);

            await _motionRepository.AddMotionEventAsync(motionEvent);

            // New Logic: Check lock state and trigger notification if necessary
            var lockState = await _trackerRepository.FetchLockStateAsync(trackerId);
            _logger.LogInformation("Tracker '{TrackerId}' has lock state '{LockState}'.", trackerId, lockState);

            if (lockState != null && lockState.Equals("locked", StringComparison.OrdinalIgnoreCase) && motionDetected)
            {
                // Create a notification
                var notification = new Notification
                {
                    TrackerId = trackerId,
                    Type = "MotionDetected",
                    Message = $"Motion detected on locked tracker {trackerId}.",
                    Timestamp = DateTime.UtcNow
                };

                await _notificationManager.CreateNotificationAsync(notification);
                _logger.LogInformation("Notification triggered for Tracker '{TrackerId}' due to motion detection.", trackerId);
            }
        }

        /// <summary>
        /// Retrieves all motion detection events for a Tracker.
        /// </summary>
        public async Task<List<Motion>> FetchMotionEventsAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.", nameof(trackerId));
            }

            _logger.LogInformation("Fetching motion events for Tracker '{TrackerId}'.", trackerId);

            var motionEvents = await _motionRepository.GetMotionEventsByTrackerIdAsync(trackerId);

            _logger.LogInformation("Fetched {Count} motion events for Tracker '{TrackerId}'.", motionEvents.Count, trackerId);

            return motionEvents;
        }
    }
}
