// File: ArgusService/Controllers/LocationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // ILocationManager interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper; // Add this
using ArgusService.Models; // Ensure this is included for the Location model

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationManager _locationManager;
        private readonly ILogger<LocationsController> _logger;
        private readonly IMapper _mapper; // Inject IMapper

        public LocationsController(ILocationManager locationManager, ILogger<LocationsController> logger, IMapper mapper)
        {
            _locationManager = locationManager;
            _logger = logger;
            _mapper = mapper; // Assign IMapper
        }

        /// <summary>
        /// Saves location data.
        /// Example body:
        /// {
        ///   "trackerId": "Tracker001",
        ///   "latitude": 40.7128,
        ///   "longitude": -74.0060,
        ///   "timestamp": "2025-01-17T06:00:00Z"
        /// }
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> AddLocation([FromBody] LocationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid LocationRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Model
                var location = _mapper.Map<Location>(dto);

                await _locationManager.SaveLocationAsync(location.TrackerId, location.Latitude, location.Longitude, location.Timestamp);
                _logger.LogInformation("Location added for Tracker '{TrackerId}'.", location.TrackerId);
                return Ok(new { Message = "Location added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding location for Tracker '{TrackerId}'.", dto.TrackerId);
                return StatusCode(500, new { Message = "An error occurred while adding the location." });
            }
        }

        /// <summary>
        /// Fetches location history for a tracker.
        /// Example GET: /api/locations/{trackerId}
        /// </summary>
        [HttpGet("{trackerId}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetLocationHistory(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch location history.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var locations = await _locationManager.GetLocationHistoryAsync(trackerId);
                _logger.LogInformation("Fetched location history for Tracker '{TrackerId}'.", trackerId);
                return Ok(locations);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while fetching location history for Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching location history for Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Exports location history in CSV or PDF format.
        /// Example GET: /api/locations/export/{trackerId}?format=csv
        /// </summary>
        [HttpGet("export/{trackerId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ExportLocationHistory(string trackerId, [FromQuery] string format)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to export location history.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            if (string.IsNullOrEmpty(format))
            {
                _logger.LogWarning("Format query parameter is required for exporting location history.");
                return BadRequest(new { Message = "Format query parameter is required." });
            }

            try
            {
                var fileData = await _locationManager.ExportLocationHistoryAsync(trackerId, format);
                var contentType = format.ToLower() switch
                {
                    "csv" => "text/csv",
                    "pdf" => "application/pdf",
                    _ => "application/octet-stream"
                };
                var fileExtension = format.ToLower();
                var fileName = $"LocationHistory_{trackerId}.{fileExtension}";

                _logger.LogInformation("Exported location history for Tracker '{TrackerId}' in '{Format}' format.", trackerId, format);
                return File(fileData, contentType, fileName);
            }
            catch (NotImplementedException notImplEx)
            {
                _logger.LogError(notImplEx, "Export format '{Format}' not implemented.", format);
                return StatusCode(501, new { Message = notImplEx.Message });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while exporting location history for Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting location history for Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
