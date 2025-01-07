using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class TrackerRepository : ITracker
    {
        private readonly MyDbContext _context;

        public TrackerRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registers a new Tracker or Lock in the system.
        /// </summary>
        public async Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null)
        {
            if (deviceType.ToLower() == "tracker")
            {
                var tracker = new Tracker
                {
                    TrackerId = deviceId,
                    FirebaseUID = null,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.Trackers.AddAsync(tracker);
            }
            else if (deviceType.ToLower() == "lock")
            {
                var lockModel = new Lock
                {
                    DeviceId = deviceId,
                    AttachedTrackerId = attachedTrackerId,
                    LastUpdated = DateTime.UtcNow,
                    Status = "locked" // Default state
                };
                await _context.Locks.AddAsync(lockModel);
            }
            else
            {
                throw new ArgumentException("Invalid device type. Allowed types are 'tracker' or 'lock'.");
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all devices (Trackers and Locks) from the database.
        /// </summary>
        public async Task<List<object>> GetAllDevicesAsync()
        {
            var trackers = await _context.Trackers.ToListAsync();
            var locks = await _context.Locks.ToListAsync();

            var allDevices = new List<object>();
            allDevices.AddRange(trackers);
            allDevices.AddRange(locks);

            return allDevices;
        }

        /// <summary>
        /// Updates the lock state of a tracker.
        /// </summary>
        public async Task UpdateLockStateAsync(string trackerId, string lockState)
        {
            var lockModel = await _context.Locks.FirstOrDefaultAsync(l => l.AttachedTrackerId == trackerId);
            if (lockModel == null)
            {
                throw new Exception($"No lock found for tracker with ID: {trackerId}");
            }

            lockModel.Status = lockState;
            lockModel.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the current lock state of a tracker.
        /// </summary>
        public async Task<string> FetchLockStateAsync(string trackerId)
        {
            var lockModel = await _context.Locks.FirstOrDefaultAsync(l => l.AttachedTrackerId == trackerId);
            if (lockModel == null)
            {
                throw new Exception($"No lock found for tracker with ID: {trackerId}");
            }

            return lockModel.Status;
        }

        /// <summary>
        /// Fetches a tracker by its ID.
        /// </summary>
        public async Task<Tracker> FetchTrackerAsync(string trackerId)
        {
            return await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == trackerId);
        }

        /// <summary>
        /// Updates a tracker in the database.
        /// </summary>
        public async Task UpdateTrackerAsync(Tracker tracker)
        {
            var existingTracker = await _context.Trackers.FirstOrDefaultAsync(t => t.TrackerId == tracker.TrackerId);
            if (existingTracker == null)
            {
                throw new Exception($"Tracker with ID {tracker.TrackerId} not found.");
            }

            // Update the tracker properties
            existingTracker.FirebaseUID = tracker.FirebaseUID;
            existingTracker.Email = tracker.Email;
            existingTracker.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
