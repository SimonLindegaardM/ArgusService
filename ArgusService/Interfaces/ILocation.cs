using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface ILocation
    {
        Task AddLocationAsync(Location location);
        Task<List<Location>> GetLocationHistoryAsync(string trackerId);
    }
}
