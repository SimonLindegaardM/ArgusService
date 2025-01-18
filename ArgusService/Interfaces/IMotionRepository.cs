using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Motion repository operations.
    /// </summary>
    public interface IMotionRepository
    {
        /// <summary>
        /// Adds a new motion detection event.
        /// </summary>
        Task AddMotionEventAsync(Motion motionEvent);

        /// <summary>
        /// Retrieves all motion events for a specific Tracker.
        /// </summary>
        Task<List<Motion>> GetMotionEventsByTrackerIdAsync(string trackerId);
    }
}
