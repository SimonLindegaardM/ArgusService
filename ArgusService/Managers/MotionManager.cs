using ArgusService.Interfaces;
using ArgusService.Models;
using ArgusService.Repositories;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ArgusService.Managers
{
    public class MotionManager : IMotionManager
    {
        private readonly MotionRepository _motionRepository;

        public MotionManager(MotionRepository motionRepository)
        {
            _motionRepository = motionRepository;
        }

        /// <summary>
        /// Logs a motion detection event for a tracker.
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
        /// Retrieves motion detection logs for a tracker.
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
