// File: ArgusService/Repositories/TrackerRepository.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient; // Ensure this using directive is present

namespace ArgusService.Repositories
{
    /// <summary>
    /// Repository for Tracker-related data operations.
    /// </summary>
    public class TrackerRepository : ITrackerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrackerRepository> _logger;

        public TrackerRepository(ApplicationDbContext context, ILogger<TrackerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new Tracker device. 
        /// If deviceType != "tracker", throws an exception.
        /// </summary>
        public async Task RegisterDeviceAsync(string deviceId, string deviceType)
        {
            _logger.LogInformation("Attempting to register Tracker '{DeviceId}' of type '{DeviceType}'.", deviceId, deviceType);

            try
            {
                if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(deviceType))
                {
                    _logger.LogWarning("Registration failed: Device ID or Device Type is null or empty.");
                    throw new ArgumentException("Device ID and Device Type cannot be null or empty.");
                }

                if (deviceType.ToLower() != "tracker")
                {
                    _logger.LogWarning("Registration failed: Device Type '{DeviceType}' is not 'tracker'.", deviceType);
                    throw new InvalidOperationException("Registering a Lock is not handled by TrackerRepository.");
                }

                // Check if tracker already exists
                var existingTracker = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == deviceId);
                if (existingTracker != null)
                {
                    _logger.LogWarning("Registration failed: Tracker with ID '{DeviceId}' already exists.", deviceId);
                    throw new InvalidOperationException($"Tracker with ID '{deviceId}' already exists.");
                }

                // Create a new Tracker record with some defaults
                var tracker = new Tracker
                {
                    TrackerId = deviceId,
                    FirebaseUID = null,
                    Email = "default@example.com",
                    MqttUsername = "defaultUser",
                    MqttPassword = "defaultPass",
                    BrokerUrl = "http://broker.example.com",
                    Port = 1883,
                    LockState = "locked",
                    DesiredLockState = "locked",
                    LastKnownLocation = null,
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow // Initialized CreatedAt
                };

                await _context.Trackers.AddAsync(tracker);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully registered Tracker '{DeviceId}'.", deviceId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while registering Tracker '{DeviceId}'.", deviceId);
                throw; // Rethrow to allow higher layers to handle if necessary
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering Tracker '{DeviceId}'.", deviceId);
                throw;
            }
        }

        /// <summary>
        /// Fetches a Tracker by its ID.
        /// </summary>
        public async Task<Tracker> FetchTrackerAsync(string trackerId)
        {
            _logger.LogInformation("Fetching Tracker with ID '{TrackerId}'.", trackerId);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("Fetch failed: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                var tracker = await _context.Trackers
                    .Include(t => t.Locations) // Include related data if necessary
                    .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

                if (tracker == null)
                {
                    _logger.LogWarning("Fetch failed: Tracker with ID '{TrackerId}' not found.", trackerId);
                    return null;
                }

                _logger.LogInformation("Successfully fetched Tracker '{TrackerId}'.", trackerId);
                return tracker;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while fetching Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing Tracker (including lockState, desiredLockState).
        /// </summary>
        public async Task UpdateTrackerAsync(Tracker tracker)
        {
            _logger.LogInformation("Updating Tracker '{TrackerId}'.", tracker.TrackerId);

            try
            {
                if (tracker == null || string.IsNullOrEmpty(tracker.TrackerId))
                {
                    _logger.LogWarning("Update failed: Tracker or TrackerId is null or empty.");
                    throw new ArgumentException("Tracker or TrackerId cannot be null or empty.");
                }

                var existing = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == tracker.TrackerId);
                if (existing == null)
                {
                    _logger.LogWarning("Update failed: Tracker with ID '{TrackerId}' not found.", tracker.TrackerId);
                    throw new Exception($"Tracker with ID {tracker.TrackerId} not found.");
                }

                // Overwrite existing fields
                existing.FirebaseUID = tracker.FirebaseUID;
                existing.Email = tracker.Email;
                existing.MqttUsername = tracker.MqttUsername;
                existing.MqttPassword = tracker.MqttPassword;
                existing.BrokerUrl = tracker.BrokerUrl;
                existing.Port = tracker.Port;
                existing.LockState = tracker.LockState.ToLower(); // Ensure consistency
                existing.DesiredLockState = tracker.DesiredLockState.ToLower();
                existing.LastKnownLocation = tracker.LastKnownLocation;
                existing.LastUpdated = DateTime.UtcNow;

                _context.Trackers.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated Tracker '{TrackerId}'.", tracker.TrackerId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while updating Tracker '{TrackerId}'.", tracker.TrackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Tracker '{TrackerId}'.", tracker.TrackerId);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all devices (Trackers).
        /// </summary>
        public async Task<List<Tracker>> GetAllDevicesAsync()
        {
            _logger.LogInformation("Fetching all Tracker devices.");

            try
            {
                var trackers = await _context.Trackers
                                             .AsNoTracking()
                                             .ToListAsync();

                if (trackers == null)
                {
                    _logger.LogWarning("Fetch all devices: Trackers list is null.");
                    return new List<Tracker>(); // Return empty list instead of null
                }

                _logger.LogInformation("Successfully fetched {Count} Tracker devices.", trackers.Count);
                return trackers;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while fetching all Trackers.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all Trackers.");
                throw;
            }
        }

        /// <summary>
        /// Updates the lock state of a Tracker.
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            _logger.LogInformation("Updating lock state for Tracker '{TrackerId}' to '{LockState}'.", trackerId, lockState);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("Update lock state failed: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                if (string.IsNullOrEmpty(lockState) || (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked"))
                {
                    _logger.LogWarning("Update lock state failed: Invalid LockState '{LockState}' provided for Tracker '{TrackerId}'.", lockState, trackerId);
                    throw new ArgumentException("LockState must be either 'locked' or 'unlocked'.");
                }

                var tracker = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == trackerId);
                if (tracker == null)
                {
                    _logger.LogWarning("Update lock state failed: Tracker with ID '{TrackerId}' not found.", trackerId);
                    throw new Exception($"Tracker with ID {trackerId} not found.");
                }

                // Update the fields
                tracker.LockState = lockState.ToLower(); // Ensure consistency
                tracker.DesiredLockState = lockState.ToLower();
                tracker.LastUpdated = DateTime.UtcNow;

                _context.Trackers.Update(tracker);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated lock state for Tracker '{TrackerId}' to '{LockState}'.", trackerId, lockState);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while updating lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }

        /// <summary>
        /// Fetches the current lock state (LockState) of a Tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            _logger.LogInformation("Fetching lock state for Tracker '{TrackerId}'.", trackerId);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("Fetch lock state failed: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                var tracker = await _context.Trackers
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

                if (tracker == null)
                {
                    _logger.LogWarning("Fetch lock state failed: Tracker with ID '{TrackerId}' not found.", trackerId);
                    return null;
                }

                _logger.LogInformation("Tracker '{TrackerId}' has lock state '{LockState}'.", trackerId, tracker.LockState);
                return tracker.LockState;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while fetching lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching lock state for Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }

        /// <summary>
        /// Deletes a Tracker and all associated Locations.
        /// </summary>
        public async Task DeleteTrackerAsync(string trackerId)
        {
            _logger.LogInformation("Deleting Tracker '{TrackerId}' and associated data.", trackerId);

            try
            {
                if (string.IsNullOrEmpty(trackerId))
                {
                    _logger.LogWarning("Delete Tracker failed: Tracker ID is null or empty.");
                    throw new ArgumentException("Tracker ID cannot be null or empty.");
                }

                var tracker = await _context.Trackers
                                            .Include(t => t.Locations)
                                            .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

                if (tracker == null)
                {
                    _logger.LogWarning("Delete Tracker failed: Tracker with ID '{TrackerId}' does not exist.", trackerId);
                    throw new InvalidOperationException($"Tracker with ID {trackerId} does not exist.");
                }

                // Remove associated Locations
                if (tracker.Locations != null && tracker.Locations.Count > 0)
                {
                    _context.Locations.RemoveRange(tracker.Locations);
                    _logger.LogInformation("Removed {Count} locations associated with Tracker '{TrackerId}'.", tracker.Locations.Count, trackerId);
                }

                _context.Trackers.Remove(tracker);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted Tracker '{TrackerId}' and all associated data.", trackerId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception occurred while deleting Tracker '{TrackerId}'.", trackerId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Tracker '{TrackerId}'.", trackerId);
                throw;
            }
        }
    }
}
