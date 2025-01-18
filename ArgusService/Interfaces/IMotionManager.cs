// File: ArgusService/Interfaces/IMotionManager.cs

using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Motion manager operations.
    /// </summary>
    public interface IMotionManager
    {
        /// <summary>
        /// Logs a motion detection event for a Tracker.
        /// </summary>
        Task LogMotionEventAsync(string trackerId, bool motionDetected, DateTime timestamp);

        /// <summary>
        /// Retrieves all motion detection events for a Tracker.
        /// </summary>
        Task<List<Motion>> FetchMotionEventsAsync(string trackerId);
    }
}
