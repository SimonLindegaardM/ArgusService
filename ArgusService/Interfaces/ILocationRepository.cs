using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Location repository operations (data access).
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Adds a new location entry to the database.
        /// </summary>
        Task AddLocationAsync(Location location);

        /// <summary>
        /// Retrieves location history for a specific tracker.
        /// </summary>
        Task<List<Location>> GetLocationHistoryAsync(string trackerId);

        /// <summary>
        /// Exports location history for a specific tracker in CSV or PDF format.
        /// </summary>
        Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format);
    }
}
