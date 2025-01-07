using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api")]
    public class LocationController : ControllerBase
    {
        private readonly LocationManager _locationManager;

        public LocationController(LocationManager locationManager)
        {
            _locationManager = locationManager;
        }

        /// <summary>
        /// Saves a location update for a tracker.
        /// </summary>
        [HttpPost("locations")]
        public async Task<IActionResult> SaveLocation([FromBody] SaveLocationRequest request)
        {
            try
            {
                await _locationManager.SaveLocationAsync(request.TrackerId, request.Latitude, request.Longitude);
                return Ok(new { Message = "Location saved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches location history for a tracker.
        /// </summary>
        [HttpGet("locations/{trackerId}")]
        public async Task<IActionResult> GetLocationHistory(string trackerId)
        {
            try
            {
                var locations = await _locationManager.GetLocationHistoryAsync(trackerId);
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Exports location history in CSV or PDF format.
        /// </summary>
        [HttpGet("locations/export/{trackerId}")]
        public async Task<IActionResult> ExportLocationHistory(string trackerId, [FromQuery] string format)
        {
            try
            {
                var fileData = await _locationManager.ExportLocationHistoryAsync(trackerId, format);
                var contentType = format.ToLower() == "csv" ? "text/csv" : "application/pdf";
                var fileName = $"LocationHistory_{trackerId}.{format.ToLower()}";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class SaveLocationRequest
    {
        public string TrackerId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
