using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface ILocationManager
    {
        Task SaveLocationAsync(string trackerId, double latitude, double longitude);
        Task<List<Location>> GetLocationHistoryAsync(string trackerId);
        Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format);
    }
}
