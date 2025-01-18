using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;

namespace ArgusService.Managers
{
    /// <summary>
    /// Manager for Location-related business logic.
    /// </summary>
    public class LocationManager : ILocationManager
    {
        private readonly ILocationRepository _locationRepository;

        public LocationManager(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        /// <summary>
        /// Saves a location update for a tracker.
        /// If 'timestamp' is null, uses DateTime.UtcNow.
        /// </summary>
        public async Task SaveLocationAsync(string trackerId, double latitude, double longitude, DateTime? timestamp = null)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            var location = new Location
            {
                TrackerId = trackerId,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = timestamp ?? DateTime.UtcNow
            };

            await _locationRepository.AddLocationAsync(location);
        }

        /// <summary>
        /// Retrieves the location history for a given tracker.
        /// </summary>
        public async Task<List<Location>> GetLocationHistoryAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            return await _locationRepository.GetLocationHistoryAsync(trackerId);
        }

        /// <summary>
        /// Exports the location history in CSV or PDF format.
        /// Currently, only CSV is implemented.
        /// </summary>
        public async Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("Format cannot be null or empty.");

            return await _locationRepository.ExportLocationHistoryAsync(trackerId, format);
        }
    }
}
