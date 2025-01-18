﻿using ArgusService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArgusService.Interfaces
{
    /// <summary>
    /// Interface for Location manager operations (business logic).
    /// </summary>
    public interface ILocationManager
    {
        /// <summary>
        /// Saves a location update for a tracker.
        /// </summary>
        Task SaveLocationAsync(string trackerId, double latitude, double longitude, DateTime? timestamp = null);

        /// <summary>
        /// Retrieves the location history for a given tracker.
        /// </summary>
        Task<List<Location>> GetLocationHistoryAsync(string trackerId);

        /// <summary>
        /// Exports the location history in CSV or PDF format.
        /// </summary>
        Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format);
    }
}
