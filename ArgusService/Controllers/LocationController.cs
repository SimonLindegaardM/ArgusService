// File: ArgusService/Controllers/LocationsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper;
using ArgusService.Models;

namespace ArgusService.Controllers
{
    /// <summary>
    /// Controller for managing Location data.
    /// </summary>
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationManager _locationManager;
        private readonly ILogger<LocationsController> _logger;
        private readonly IMapper _mapper;

        public LocationsController(ILocationManager locationManager, ILogger<LocationsController> logger, IMapper mapper)
        {
            _locationManager = locationManager;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Saves location data.
        /// </summary>
        /// <param name="dto">The location details.</param>
        /// <returns>Result of the save operation.</returns>
        /// <response code="200">Location added successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
        /// </summary>
        /// <param name="trackerId">The ID of the Tracker.</param>
        /// <returns>List of location records.</returns>
        /// <response code="200">Returns the list of locations.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
        /// </summary>
        /// <param name="trackerId">The ID of the Tracker.</param>
        /// <param name="format">The format to export (csv or pdf).</param>
        /// <returns>Exported file.</returns>
        /// <response code="200">Returns the exported file.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="501">If the export format is not implemented.</response>
        /// <response code="500">If an unexpected error occurs.</response>
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
