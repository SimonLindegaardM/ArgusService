using ArgusService.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ArgusService.Interfaces
{
    public interface IMotionManager
    {
        Task LogMotionEventAsync(string trackerId, bool motionDetected);
        Task<List<Motion>> FetchMotionEventsAsync(string trackerId);
    }
}
