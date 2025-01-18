using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.Extensions.Logging;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Tracker-related business logic.
    /// </summary>
    public class TrackerManager : ITrackerManager
    {
        private readonly ITrackerRepository _trackerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<TrackerManager> _logger;

        public TrackerManager(
            ITrackerRepository trackerRepository,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            ILogger<TrackerManager> logger)
        {
            _trackerRepository = trackerRepository;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new Tracker device only. 
        /// If deviceType == "lock", throws an exception.
        /// </summary>
        public async Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null)
        {
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(deviceType))
                throw new ArgumentException("Device ID and Device Type cannot be null or empty.");

            // This will throw an exception if deviceType == "lock"
            await _trackerRepository.RegisterDeviceAsync(deviceId, deviceType);

            _logger.LogInformation($"Device '{deviceId}' of type '{deviceType}' registered successfully.");
        }

        /// <summary>
        /// Links a Tracker to a specific Firebase user.
        /// </summary>
        public async Task LinkDeviceToUserAsync(string trackerId, string firebaseUID, string email)
        {
            // Validate the user
            var user = await _userRepository.GetUserDetailsAsync(firebaseUID);
            if (user == null || user.Email != email)
            {
                throw new Exception($"User with FirebaseUID {firebaseUID} and email {email} not found.");
            }

            // Fetch the tracker
            var tracker = await _trackerRepository.FetchTrackerAsync(trackerId);
            if (tracker == null)
            {
                throw new Exception($"Tracker with ID {trackerId} not found.");
            }

            // Link tracker to user
            tracker.FirebaseUID = firebaseUID;
            tracker.Email = email;
            await _trackerRepository.UpdateTrackerAsync(tracker);

            _logger.LogInformation($"Tracker '{trackerId}' linked to user '{email}'.");
        }

        /// <summary>
        /// Retrieves all trackers as 'devices'.
        /// </summary>
        public async Task<List<Tracker>> GetAllDevicesAsync()
        {
            _logger.LogInformation("Fetching all devices.");

            var devices = await _trackerRepository.GetAllDevicesAsync();

            if (devices == null)
            {
                _logger.LogError("Devices list is null.");
                return new List<Tracker>(); // Return empty list instead of null
            }

            _logger.LogInformation($"Fetched {devices.Count} devices.");
            return devices;
        }

        /// <summary>
        /// Updates the lock state of a Tracker (LockState, DesiredLockState).
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            if (string.IsNullOrEmpty(lockState) || (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked"))
                throw new ArgumentException("LockState must be 'locked' or 'unlocked'.");

            await _trackerRepository.UpdateLockStateAsync(trackerId, lockState);

            _logger.LogInformation($"Tracker '{trackerId}' lock state updated to '{lockState}'.");
        }

        /// <summary>
        /// Fetches the current lock state of a Tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            var lockState = await _trackerRepository.FetchLockStateAsync(trackerId);

            if (lockState == null)
            {
                _logger.LogWarning($"Lock state for Tracker '{trackerId}' is null.");
            }

            return lockState;
        }

        /// <summary>
        /// Deletes a Tracker and all associated data (e.g., Locations).
        /// </summary>
        public async Task DeleteTrackerAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            await _trackerRepository.DeleteTrackerAsync(trackerId);

            _logger.LogInformation($"Tracker '{trackerId}' and all associated data deleted successfully.");
        }
    }
}
