using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Registering new Tracker '{DeviceId}' of type '{DeviceType}'.", deviceId, deviceType);

            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(deviceType))
            {
                _logger.LogWarning("Device ID or Device Type is null or empty.");
                throw new ArgumentException("Device ID and Device Type cannot be null or empty.");
            }

            if (deviceType.ToLower() != "tracker")
            {
                _logger.LogWarning("Attempted to register a device of type '{DeviceType}', which is not 'tracker'.", deviceType);
                throw new InvalidOperationException("Registering a Lock is not handled by TrackerRepository.");
            }

            // Check if tracker already exists
            var existingTracker = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == deviceId);
            if (existingTracker != null)
            {
                _logger.LogWarning("Tracker with ID '{DeviceId}' already exists.", deviceId);
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
                LastUpdated = DateTime.UtcNow
            };

            await _context.Trackers.AddAsync(tracker);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tracker '{DeviceId}' registered successfully.", deviceId);
        }

        /// <summary>
        /// Fetches a Tracker by its ID.
        /// </summary>
        public async Task<Tracker> FetchTrackerAsync(string trackerId)
        {
            _logger.LogInformation("Fetching Tracker with ID '{TrackerId}'.", trackerId);

            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            var tracker = await _context.Trackers
                .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

            if (tracker == null)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' not found.", trackerId);
                return null;
            }

            _logger.LogInformation("Tracker '{TrackerId}' fetched successfully.", trackerId);
            return tracker;
        }

        /// <summary>
        /// Updates an existing Tracker (including lockState, desiredLockState).
        /// </summary>
        public async Task UpdateTrackerAsync(Tracker tracker)
        {
            _logger.LogInformation("Updating Tracker '{TrackerId}'.", tracker.TrackerId);

            if (tracker == null || string.IsNullOrEmpty(tracker.TrackerId))
            {
                _logger.LogWarning("Tracker or TrackerId is null or empty.");
                throw new ArgumentException("Tracker or TrackerId cannot be null or empty.");
            }

            var existing = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == tracker.TrackerId);
            if (existing == null)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' not found.", tracker.TrackerId);
                throw new Exception($"Tracker with ID {tracker.TrackerId} not found.");
            }

            // Overwrite existing fields
            existing.FirebaseUID = tracker.FirebaseUID;
            existing.Email = tracker.Email;
            existing.MqttUsername = tracker.MqttUsername;
            existing.MqttPassword = tracker.MqttPassword;
            existing.BrokerUrl = tracker.BrokerUrl;
            existing.Port = tracker.Port;
            existing.LockState = tracker.LockState;
            existing.DesiredLockState = tracker.DesiredLockState;
            existing.LastKnownLocation = tracker.LastKnownLocation;
            existing.LastUpdated = DateTime.UtcNow;

            _context.Trackers.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tracker '{TrackerId}' updated successfully.", tracker.TrackerId);
        }

        /// <summary>
        /// Retrieves all devices (Trackers).
        /// </summary>
        public async Task<List<Tracker>> GetAllDevicesAsync()
        {
            _logger.LogInformation("Fetching all Tracker devices.");

            var trackers = await _context.Trackers
                                         .AsNoTracking()
                                         .ToListAsync();

            if (trackers == null)
            {
                _logger.LogError("Trackers list is null.");
                return new List<Tracker>(); // Return empty list instead of null
            }

            _logger.LogInformation("Fetched {Count} Tracker devices.", trackers.Count);
            return trackers;
        }

        /// <summary>
        /// Updates the lock state of a Tracker.
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            _logger.LogInformation("Updating lock state for Tracker '{TrackerId}' to '{LockState}'.", trackerId, lockState);

            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            if (string.IsNullOrEmpty(lockState) || (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked"))
            {
                _logger.LogWarning("Invalid LockState '{LockState}' provided for Tracker '{TrackerId}'.", lockState, trackerId);
                throw new ArgumentException("LockState must be either 'locked' or 'unlocked'.");
            }

            var tracker = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == trackerId);
            if (tracker == null)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' not found.", trackerId);
                throw new Exception($"Tracker with ID {trackerId} not found.");
            }

            // Update the fields
            tracker.LockState = lockState;
            tracker.DesiredLockState = lockState;
            tracker.LastUpdated = DateTime.UtcNow;

            _context.Trackers.Update(tracker);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tracker '{TrackerId}' lock state updated to '{LockState}'.", trackerId, lockState);
        }

        /// <summary>
        /// Fetches the current lock state (LockState) of a Tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            _logger.LogInformation("Fetching lock state for Tracker '{TrackerId}'.", trackerId);

            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            var tracker = await _context.Trackers
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

            if (tracker == null)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' not found.", trackerId);
                return null;
            }

            _logger.LogInformation("Tracker '{TrackerId}' has lock state '{LockState}'.", trackerId, tracker.LockState);
            return tracker.LockState;
        }

        /// <summary>
        /// Deletes a Tracker and all associated Locations.
        /// </summary>
        public async Task DeleteTrackerAsync(string trackerId)
        {
            _logger.LogInformation("Deleting Tracker '{TrackerId}' and associated data.", trackerId);

            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            var tracker = await _context.Trackers
                                        .Include(t => t.Locations)
                                        .FirstOrDefaultAsync(t => t.TrackerId == trackerId);

            if (tracker == null)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' does not exist.", trackerId);
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

            _logger.LogInformation("Tracker '{TrackerId}' deleted successfully.", trackerId);
        }
    }
}
