using ArgusService.Interfaces;
using ArgusService.Models;
using ArgusService.Repositories;

namespace ArgusService.Managers
{
    public class LockManager : ILockManager
    {
        private readonly LockRepository _lockRepository;

        public LockManager(LockRepository lockRepository)
        {
            _lockRepository = lockRepository;
        }

        public async Task RegisterLockToTrackerAsync(string lockId, string trackerId)
        {
            if (string.IsNullOrEmpty(lockId) || string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Lock ID and Tracker ID cannot be null or empty.");
            }

            await _lockRepository.RegisterLockAsync(lockId, trackerId);
        }

        public async Task<List<Lock>> FetchLocksByTrackerIdAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            return await _lockRepository.GetLocksByTrackerIdAsync(trackerId);
        }

        public async Task UpdateLockStateAsync(string lockId, string lockState)
        {
            if (string.IsNullOrEmpty(lockId))
            {
                throw new ArgumentException("Lock ID cannot be null or empty.");
            }

            if (lockState.ToLower() != "locked" && lockState.ToLower() != "unlocked")
            {
                throw new ArgumentException("Invalid lock state. Allowed values are 'locked' or 'unlocked'.");
            }

            await _lockRepository.UpdateLockStateAsync(lockId, lockState);
        }
    }
}
