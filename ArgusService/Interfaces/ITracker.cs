﻿using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface ITracker
    {
        Task RegisterDeviceAsync(string deviceId, string deviceType, string attachedTrackerId = null);
        Task<List<object>> GetAllDevicesAsync();
        Task UpdateLockStateAsync(string trackerId, string lockState);
        Task<string> FetchLockStateAsync(string trackerId);
        Task<Tracker> FetchTrackerAsync(string trackerId);
        Task UpdateTrackerAsync(Tracker tracker);
    }
}
