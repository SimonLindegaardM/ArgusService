namespace ArgusService.Interfaces
{
    public interface ITrackerManager
    {
        Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null);
        Task LinkDeviceToUserAsync(string trackerId, string firebaseUID, string email);
        Task<List<object>> FetchAllDevicesAsync();
        Task UpdateLockStateAsync(string trackerId, string lockState);
        Task<string> FetchLockStateAsync(string trackerId);
    }
}
