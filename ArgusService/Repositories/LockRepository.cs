using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgusService.Data;      // Use your custom DbContext namespace
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgusService.Repositories
{
    /// <summary>
    /// Implementation of lock-related data access methods.
    /// </summary>
    public class LockRepository : ILockRepository
    {
        private readonly ApplicationDbContext _context;

        public LockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registers a new lock device and associates it with a tracker.
        /// </summary>
        public async Task RegisterLockAsync(string lockId, string trackerId)
        {
            if (string.IsNullOrEmpty(lockId) || string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Lock ID and Tracker ID cannot be null or empty.");
            }

            // Check if lock already exists
            var existingLock = await _context.Locks.FirstOrDefaultAsync(l => l.LockId == lockId);
            if (existingLock != null)
            {
                throw new InvalidOperationException($"Lock with ID '{lockId}' already exists.");
            }

            // Check if tracker exists
            var trackerExists = await _context.Trackers.AnyAsync(t => t.TrackerId == trackerId);
            if (!trackerExists)
            {
                throw new InvalidOperationException($"Tracker with ID '{trackerId}' does not exist.");
            }

            var newLock = new Lock
            {
                LockId = lockId,
                TrackerId = trackerId,
                // The following may be assigned externally or default
                FirebaseUID = null,
                Email = null,
                Status = "unlocked", // Default initial state
                LastUpdated = DateTime.UtcNow
            };

            _context.Locks.Add(newLock);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the lock state ("locked"/"unlocked") for a given lock ID.
        /// </summary>
        public async Task UpdateLockStateAsync(string lockId, string lockState)
        {
            var lockEntity = await _context.Locks.FirstOrDefaultAsync(l => l.LockId == lockId);
            if (lockEntity == null)
            {
                throw new InvalidOperationException($"Lock with ID '{lockId}' not found.");
            }

            lockEntity.Status = lockState;
            lockEntity.LastUpdated = DateTime.UtcNow;

            _context.Locks.Update(lockEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all locks attached to a particular tracker.
        /// </summary>
        public async Task<List<Lock>> GetLocksByTrackerIdAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            return await _context.Locks
                .Where(l => l.TrackerId == trackerId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all locks in the system.
        /// </summary>
        public async Task<List<Lock>> GetAllLocksAsync()
        {
            return await _context.Locks
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific lock by its lock ID.
        /// </summary>
        public async Task<Lock> GetLockByIdAsync(string lockId)
        {
            if (string.IsNullOrEmpty(lockId))
                throw new ArgumentException("Lock ID cannot be null or empty.");

            return await _context.Locks
                .FirstOrDefaultAsync(l => l.LockId == lockId);
        }
    }
}
