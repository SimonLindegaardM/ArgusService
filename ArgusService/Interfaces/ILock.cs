using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface ILock
    {
        Task RegisterLockAsync(string deviceId, string attachedTrackerId);
        Task UpdateLockStateAsync(string deviceId, string lockState);
        Task<List<Lock>> GetLocksByTrackerIdAsync(string trackerId);
    }
}
