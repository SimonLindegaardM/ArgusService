using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Lock repository operations (data access).
    /// </summary>
    public interface ILockRepository
    {
        /// <summary>
        /// Registers a new lock device and associates it with a tracker.
        /// </summary>
        Task RegisterLockAsync(string lockId, string trackerId);

        /// <summary>
        /// Updates the lock state ("locked"/"unlocked") for a given lock ID.
        /// </summary>
        Task UpdateLockStateAsync(string lockId, string lockState);

        /// <summary>
        /// Retrieves all locks attached to a particular tracker.
        /// </summary>
        Task<List<Lock>> GetLocksByTrackerIdAsync(string trackerId);

        /// <summary>
        /// Retrieves all locks in the system.
        /// </summary>
        Task<List<Lock>> GetAllLocksAsync();

        /// <summary>
        /// Retrieves a specific lock by its lock ID.
        /// </summary>
        Task<Lock> GetLockByIdAsync(string lockId);
    }
}
