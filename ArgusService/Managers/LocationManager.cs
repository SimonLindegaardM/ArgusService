using ArgusService.Interfaces;
using ArgusService.Models;
using ArgusService.Repositories;

namespace ArgusService.Managers
{
    public class LocationManager : ILocationManager
    {
        private readonly LocationRepository _locationRepository;

        public LocationManager(LocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task SaveLocationAsync(string trackerId, double latitude, double longitude)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            var location = new Location
            {
                TrackerId = trackerId,
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow
            };

            await _locationRepository.AddLocationAsync(location);
        }

        public async Task<List<Location>> GetLocationHistoryAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            return await _locationRepository.GetLocationHistoryAsync(trackerId);
        }

        public async Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format)
        {
            if (string.IsNullOrEmpty(trackerId) || string.IsNullOrEmpty(format))
            {
                throw new ArgumentException("Tracker ID and format cannot be null or empty.");
            }

            var locations = await _locationRepository.GetLocationHistoryAsync(trackerId);

            if (format.ToLower() == "csv")
            {
                var csvData = "Latitude,Longitude,Timestamp\n" +
                              string.Join("\n", locations.Select(l => $"{l.Latitude},{l.Longitude},{l.Timestamp:O}"));
                return System.Text.Encoding.UTF8.GetBytes(csvData);
            }
            else if (format.ToLower() == "pdf")
            {
                throw new NotImplementedException("PDF export is not yet implemented.");
            }
            else
            {
                throw new ArgumentException("Invalid format. Allowed values are 'csv' or 'pdf'.");
            }
        }
    }
}
