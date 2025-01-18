// File: ArgusService/Controllers/MotionsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // IMotionRepository interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/motions")]
    public class MotionsController : ControllerBase
    {
        private readonly IMotionRepository _motionRepository;
        private readonly ILogger<MotionsController> _logger;

        public MotionsController(IMotionRepository motionRepository, ILogger<MotionsController> logger)
        {
            _motionRepository = motionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Saves motion event data.
        /// Example body:
        /// {
        ///   "trackerId": "Tracker001",
        ///   "motionDetected": true,
        ///   "timestamp": "2025-01-17T06:00:00Z"
        /// }
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> AddMotionEvent([FromBody] MotionRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid MotionRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _motionRepository.AddMotionEventAsync(dto);
                _logger.LogInformation("Motion event logged for Tracker '{TrackerId}'.", dto.TrackerId);
                return Ok(new { Message = "Motion event added successfully." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while logging motion event for Tracker '{TrackerId}'.", dto.TrackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "Invalid operation while logging motion event for Tracker '{TrackerId}'.", dto.TrackerId);
                return BadRequest(new { Message = invOpEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging motion event for Tracker '{TrackerId}'.", dto.TrackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches motion detection events for a Tracker.
        /// Example GET: /api/motions/{trackerId}
        /// </summary>
        [HttpGet("{trackerId}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetMotionEvents(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch motion events.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var motionEvents = await _motionRepository.FetchMotionEventsAsync(trackerId);
                _logger.LogInformation("Fetched motion events for Tracker '{TrackerId}'.", trackerId);
                return Ok(motionEvents);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while fetching motion events for Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching motion events for Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
