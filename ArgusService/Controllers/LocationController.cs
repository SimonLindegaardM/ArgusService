// File: ArgusService/Controllers/LocationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // ILocationRepository interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocationRepository locationRepository, ILogger<LocationsController> logger)
        {
            _locationRepository = locationRepository;
            _logger = logger;
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
        ///[Authorize(Roles = "admin,user")]
        public async Task<IActionResult> AddLocation([FromBody] LocationRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid LocationRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _locationRepository.AddLocationAsync(dto);
                _logger.LogInformation("Location added for Tracker '{TrackerId}'.", dto.TrackerId);
                return Ok(new { message = "Location added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding location for Tracker '{TrackerId}'.", dto.TrackerId);
                return StatusCode(500, new { message = "An error occurred while adding the location." });
            }
        }

        /// <summary>
        /// Fetches location history for a tracker.
        /// Example GET: /api/locations/{trackerId}
        /// </summary>
        [HttpGet("{trackerId}")]
        ///[Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetLocationHistory(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch location history.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var locations = await _locationRepository.GetLocationHistoryAsync(trackerId);
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
        ///[Authorize(Roles = "admin")]
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
                var fileData = await _locationRepository.ExportLocationHistoryAsync(trackerId, format);
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
