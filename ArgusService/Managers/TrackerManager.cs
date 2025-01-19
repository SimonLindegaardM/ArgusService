// File: ArgusService/Managers/TrackerManager.cs

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.Data.SqlClient;
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
        private readonly INotificationManager _notificationManager; // Added
        private readonly ILogger<TrackerManager> _logger;

        public TrackerManager(
            ITrackerRepository trackerRepository,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
            INotificationManager notificationManager, // Added
            ILogger<TrackerManager> logger)
        {
            _trackerRepository = trackerRepository;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _notificationManager = notificationManager; // Added
            _logger = logger;
        }

        /// <summary>
        /// Registers a new Tracker device only. 
        /// If deviceType == "lock", throws an exception.
        /// </summary>
        public async Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null)
        {
            _logger.LogInformation("TrackerManager: Registering device '{DeviceId}' of type '{DeviceType}'.", deviceId, deviceType);

            try
            {
                if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(deviceType))
                {
                    _logger.LogWarning("TrackerManager: Device ID or Device Type is null or empty.");
                    throw new ArgumentException("Device ID and Device Type cannot be null or empty.");
                }

                // This will throw an exception if deviceType == "lock"
                await _trackerRepository.RegisterDeviceAsync(deviceId, deviceType);
                _logger.LogInformation("TrackerManager: Device '{DeviceId}' of type '{DeviceType}' registered successfully.", deviceId, deviceType);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while registering device '{DeviceId}'.", deviceId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while registering device '{DeviceId}'.", deviceId);
                throw;
            }
        }

        /// <summary>
        /// Links a Tracker to a specific Firebase user.
        /// </summary>
        public async Task LinkDeviceToUserAsync(string trackerId, string firebaseUID, string email)
        {
            _logger.LogInformation("TrackerManager: Linking Tracker '{TrackerId}' to user '{Email}'.", trackerId, email);

            try
            {
                // Validate the user
                var user = await _userRepository.GetUserDetailsAsync(firebaseUID);
                if (user == null || user.Email != email)
                {
                    _logger.LogWarning("TrackerManager: User with FirebaseUID '{FirebaseUID}' and email '{Email}' not found.", firebaseUID, email);
                    throw new Exception($"User with FirebaseUID '{firebaseUID}' and email '{email}' not found.");
                }

                // Fetch the tracker
                var tracker = await _trackerRepository.FetchTrackerAsync(trackerId);
                if (tracker == null)
                {
                    _logger.LogWarning("TrackerManager: Tracker with ID '{TrackerId}' not found.", trackerId);
                    throw new Exception($"Tracker with ID '{trackerId}' not found.");
                }

                // Link tracker to user
                tracker.FirebaseUID = firebaseUID;
                tracker.Email = email;
                await _trackerRepository.UpdateTrackerAsync(tracker);

                _logger.LogInformation("TrackerManager: Tracker '{TrackerId}' linked to user '{Email}' successfully.", trackerId, email);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while linking Tracker '{TrackerId}' to user '{Email}'.", trackerId, email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while linking Tracker '{TrackerId}' to user '{Email}'.", trackerId, email);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all trackers as 'devices'.
        /// </summary>
        public async Task<List<Tracker>> GetAllDevicesAsync()
        {
            _logger.LogInformation("TrackerManager: Fetching all Tracker devices.");

            try
            {
                var devices = await _trackerRepository.GetAllDevicesAsync();

                if (devices == null)
                {
                    _logger.LogError("TrackerManager: Devices list is null.");
                    return new List<Tracker>(); // Return empty list instead of null
                }

                _logger.LogInformation("TrackerManager: Fetched {Count} devices.", devices.Count);
                return devices;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while fetching all devices.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while fetching all devices.");
                throw;
            }
        }

        /// <summary>
        /// Updates the lock state of a Tracker (LockState, DesiredLockState).
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            _logger.LogInformation("TrackerManager: Updating lock state for Tracker '{TrackerId}' to '{LockState}'.", trackerId, lockState);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("TrackerManager: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                if (string.IsNullOrEmpty(lockState) || (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked"))
                {
                    _logger.LogWarning("TrackerManager: Invalid LockState '{LockState}' provided for Tracker '{TrackerId}'.", lockState, trackerId);
                    throw new ArgumentException("LockState must be 'locked' or 'unlocked'.");
                }

                await _trackerRepository.UpdateLockStateAsync(trackerId, lockState);
                _logger.LogInformation("TrackerManager: Tracker '{TrackerId}' lock state updated to '{LockState}'.", trackerId, lockState);

                // New Logic: Trigger notification for lock state change
                var notificationMessage = lockState.Equals("locked", StringComparison.OrdinalIgnoreCase)
                    ? $"Tracker {trackerId} has been locked."
                    : $"Tracker {trackerId} has been unlocked.";

                var notification = new Notification
                {
                    TrackerId = trackerId,
                    Type = "LockStateChanged",
                    Message = notificationMessage,
                    Timestamp = DateTime.UtcNow
                };

                await _notificationManager.CreateNotificationAsync(notification);
                _logger.LogInformation("Notification triggered for Tracker '{TrackerId}' due to lock state change.", trackerId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while updating lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while updating lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }

        /// <summary>
        /// Fetches the current lock state of a Tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            _logger.LogInformation("TrackerManager: Fetching lock state for Tracker '{TrackerId}'.", trackerId);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("TrackerManager: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                var lockState = await _trackerRepository.FetchLockStateAsync(trackerId);

                if (lockState == null)
                {
                    _logger.LogWarning("TrackerManager: Lock state for Tracker '{TrackerId}' is null.", trackerId);
                }

                _logger.LogInformation("TrackerManager: Tracker '{TrackerId}' has lock state '{LockState}'.", trackerId, lockState);
                return lockState;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while fetching lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while fetching lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a Tracker and all associated data (e.g., Locations).
        /// </summary>
        public async Task DeleteTrackerAsync(string trackerId)
        {
            _logger.LogInformation("TrackerManager: Deleting Tracker '{TrackerId}' and all associated data.", trackerId);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("TrackerManager: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                await _trackerRepository.DeleteTrackerAsync(trackerId);
                _logger.LogInformation("TrackerManager: Tracker '{TrackerId}' and all associated data deleted successfully.", trackerId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "TrackerManager: SQL Exception occurred while deleting Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TrackerManager: An error occurred while deleting Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }
    }
}
