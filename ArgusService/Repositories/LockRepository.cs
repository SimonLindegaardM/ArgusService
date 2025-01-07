using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class LockRepository : ILock
    {
        private readonly MyDbContext _context;

        public LockRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registers a new lock and links it to a tracker.
        /// </summary>
        public async Task RegisterLockAsync(string deviceId, string attachedTrackerId)
        {
            var lockModel = new Lock
            {
                DeviceId = deviceId,
                AttachedTrackerId = attachedTrackerId,
                Status = "locked", // Default status
                LastUpdated = DateTime.UtcNow
            };

            await _context.Locks.AddAsync(lockModel);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the lock state (e.g., "locked", "unlocked").
        /// </summary>
        public async Task UpdateLockStateAsync(string deviceId, string lockState)
        {
            var lockModel = await _context.Locks.FirstOrDefaultAsync(l => l.DeviceId == deviceId);
            if (lockModel == null)
            {
                throw new Exception($"Lock with ID {deviceId} not found.");
            }

            lockModel.Status = lockState;
            lockModel.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Fetches all locks linked to a specific tracker.
        /// </summary>
        public async Task<List<Lock>> GetLocksByTrackerIdAsync(string trackerId)
        {
            return await _context.Locks.Where(l => l.AttachedTrackerId == trackerId).ToListAsync();
        }
    }
}
