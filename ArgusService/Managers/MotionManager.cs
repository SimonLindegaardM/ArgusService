using ArgusService.Interfaces;
using ArgusService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Motion-related business logic.
    /// </summary>
    public class MotionManager : IMotionManager
    {
        private readonly IMotionRepository _motionRepository;

        /// <summary>
        /// Initializes a new instance of MotionManager.
        /// </summary>
        /// <param name="motionRepository">The motion repository.</param>
        public MotionManager(IMotionRepository motionRepository)
        {
            _motionRepository = motionRepository;
        }

        /// <summary>
        /// Logs a motion detection event for a Tracker.
        /// </summary>
        public async Task LogMotionEventAsync(string trackerId, bool motionDetected)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            var motionEvent = new Motion
            {
                TrackerId = trackerId,
                MotionDetected = motionDetected,
                Timestamp = DateTime.UtcNow
            };

            await _motionRepository.AddMotionEventAsync(motionEvent);
        }

        /// <summary>
        /// Retrieves all motion detection events for a Tracker.
        /// </summary>
        public async Task<List<Motion>> FetchMotionEventsAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            return await _motionRepository.GetMotionEventsByTrackerIdAsync(trackerId);
        }
    }
}
