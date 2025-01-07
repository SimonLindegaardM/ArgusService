using ArgusService.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ArgusService.Interfaces
{
    public interface IMotion
    {
        Task AddMotionEventAsync(Motion motionEvent);
        Task<List<Motion>> GetMotionEventsByTrackerIdAsync(string trackerId);
    }
}
