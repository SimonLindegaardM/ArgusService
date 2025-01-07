using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface ILockManager
    {
        Task RegisterLockToTrackerAsync(string lockId, string trackerId);
        Task<List<Lock>> FetchLocksByTrackerIdAsync(string trackerId);
        Task UpdateLockStateAsync(string lockId, string lockState);
    }
}
