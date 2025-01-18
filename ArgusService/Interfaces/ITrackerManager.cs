// File: ArgusService/Interfaces/ITrackerManager.cs

using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Tracker manager operations.
    /// </summary>
    public interface ITrackerManager
    {
        /// <summary>
        /// Registers a new Tracker device only (locks handled by Lock side).
        /// </summary>
        Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null);

        /// <summary>
        /// Links a Tracker to a specific Firebase user.
        /// </summary>
        Task LinkDeviceToUserAsync(string trackerId, string firebaseUID, string email);

        /// <summary>
        /// Retrieves all devices (only trackers here).
        /// </summary>
        Task<List<Tracker>> GetAllDevicesAsync();

        /// <summary>
        /// Updates the lock state of a Tracker.
        /// </summary>
        Task UpdateLockStateAsync(string trackerId, string lockState);

        /// <summary>
        /// Fetches the current lock state of a Tracker.
        /// </summary>
        Task<string> FetchLockStateAsync(string trackerId);

        /// <summary>
        /// Deletes a Tracker and all associated data.
        /// </summary>
        Task DeleteTrackerAsync(string trackerId);
    }
}
