﻿using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for lock-related business logic.
    /// </summary>
    public interface ILockManager
    {
        /// <summary>
        /// Registers a new lock device and associates it with a tracker.
        /// </summary>
        Task RegisterLockToTrackerAsync(string lockId, string trackerId);

        /// <summary>
        /// Fetches all locks attached to a specific tracker.
        /// </summary>
        Task<List<Lock>> FetchLocksByTrackerIdAsync(string trackerId);

        /// <summary>
        /// Updates the state of a lock to either 'locked' or 'unlocked'.
        /// </summary>
        Task UpdateLockStateAsync(string lockId, string lockState);

        /// <summary>
        /// Retrieves all locks in the system.
        /// </summary>
        Task<List<Lock>> GetAllLocksAsync();

        /// <summary>
        /// Retrieves details of a specific lock by its ID.
        /// </summary>
        Task<Lock> GetLockByIdAsync(string lockId);
    }
}
