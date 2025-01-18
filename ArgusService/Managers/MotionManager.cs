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
        private readonly ILogger<MotionManager> _logger;

        public MotionManager(IMotionRepository motionRepository, ILogger<MotionManager> logger)
        {
            _motionRepository = motionRepository;
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
        }

        /// <summary>
        /// Retrieves all motion detection events for a Tracker.
        /// </summary>
        public async Task<List<Motion>> FetchMotionEventsAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.", nameof(trackerId));

            _logger.LogInformation("Fetching motion events for Tracker '{TrackerId}'.", trackerId);

            var motionEvents = await _motionRepository.GetMotionEventsByTrackerIdAsync(trackerId);

            _logger.LogInformation("Fetched {Count} motion events for Tracker '{TrackerId}'.", motionEvents.Count, trackerId);

            return motionEvents;
        }
    }
}
