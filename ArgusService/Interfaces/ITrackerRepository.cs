// File: ArgusService/Interfaces/ITrackerRepository.cs

using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Tracker repository operations.
    /// </summary>
    public interface ITrackerRepository
    {
        /// <summary>
        /// Registers a new Tracker device (only supports deviceType = "tracker").
        /// </summary>
        Task RegisterDeviceAsync(string deviceId, string deviceType);

        /// <summary>
        /// Fetches a Tracker by its ID.
        /// </summary>
        Task<Tracker> FetchTrackerAsync(string trackerId);

        /// <summary>
        /// Updates an existing Tracker (including lockState and desiredLockState).
        /// </summary>
        Task UpdateTrackerAsync(Tracker tracker);

        /// <summary>
        /// Retrieves all devices, i.e., Trackers.
        /// </summary>
        Task<List<Tracker>> GetAllDevicesAsync();

        /// <summary>
        /// Updates the lock state of a Tracker.
        /// </summary>
        Task UpdateLockStateAsync(string trackerId, string lockState);

        /// <summary>
        /// Fetches the current lock state of a Tracker (LockState).
        /// </summary>
        Task<string> FetchLockStateAsync(string trackerId);

        /// <summary>
        /// Deletes a Tracker and all associated Locations.
        /// </summary>
        Task DeleteTrackerAsync(string trackerId);
    }
}
