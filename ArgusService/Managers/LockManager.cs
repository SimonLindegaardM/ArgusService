using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for lock-related business logic.
    /// </summary>
    public class LockManager : ILockManager
    {
        private readonly ILockRepository _lockRepository;

        public LockManager(ILockRepository lockRepository)
        {
            _lockRepository = lockRepository;
        }

        /// <summary>
        /// Registers a lock device to a specific tracker.
        /// </summary>
        public async Task RegisterLockToTrackerAsync(string lockId, string trackerId)
        {
            if (string.IsNullOrEmpty(lockId) || string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Lock ID and Tracker ID cannot be null or empty.");
            }

            // Directly calls the repository to create the lock
            await _lockRepository.RegisterLockAsync(lockId, trackerId);
        }

        /// <summary>
        /// Fetch all locks attached to a given tracker.
        /// </summary>
        public async Task<List<Lock>> FetchLocksByTrackerIdAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            return await _lockRepository.GetLocksByTrackerIdAsync(trackerId);
        }

        /// <summary>
        /// Updates the state of a lock to either 'locked' or 'unlocked'.
        /// </summary>
        public async Task UpdateLockStateAsync(string lockId, string lockState)
        {
            if (string.IsNullOrEmpty(lockId))
            {
                throw new ArgumentException("Lock ID cannot be null or empty.");
            }

            lockState = lockState.ToLower();
            if (lockState != "locked" && lockState != "unlocked")
            {
                throw new ArgumentException("Invalid lock state. Allowed: 'locked' or 'unlocked'.");
            }

            await _lockRepository.UpdateLockStateAsync(lockId, lockState);
        }

        /// <summary>
        /// Retrieves all locks in the system.
        /// </summary>
        public async Task<List<Lock>> GetAllLocksAsync()
        {
            return await _lockRepository.GetAllLocksAsync();
        }

        /// <summary>
        /// Retrieves details of a specific lock by its ID.
        /// </summary>
        public async Task<Lock> GetLockByIdAsync(string lockId)
        {
            if (string.IsNullOrEmpty(lockId))
            {
                throw new ArgumentException("Lock ID cannot be null or empty.");
            }

            return await _lockRepository.GetLockByIdAsync(lockId);
        }
    }
}
