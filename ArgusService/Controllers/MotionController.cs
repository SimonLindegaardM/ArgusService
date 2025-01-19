// File: ArgusService/Controllers/MotionsController.cs

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
    /// Controller for managing Motion events.
    /// </summary>
    [ApiController]
    [Route("api/motions")]
    public class MotionsController : ControllerBase
    {
        private readonly IMotionManager _motionManager;
        private readonly ILogger<MotionsController> _logger;
        private readonly IMapper _mapper;

        public MotionsController(IMotionManager motionManager, ILogger<MotionsController> logger, IMapper mapper)
        {
            _motionManager = motionManager;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Saves motion event data.
        /// </summary>
        /// <param name="dto">The motion event details.</param>
        /// <returns>Result of the save operation.</returns>
        /// <response code="200">Motion event added successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost]
        ///[Authorize(Roles = "admin,user")]
        public async Task<IActionResult> AddMotionEvent([FromBody] MotionRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid MotionRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Model
                var motion = _mapper.Map<Motion>(dto);

                await _motionManager.LogMotionEventAsync(motion.TrackerId, motion.MotionDetected, motion.Timestamp);
                _logger.LogInformation("Motion event logged for Tracker '{TrackerId}'.", motion.TrackerId);
                return Ok(new { Message = "Motion event added successfully." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while logging motion event for Tracker '{TrackerId}'.", dto.TrackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging motion event for Tracker '{TrackerId}'.", dto.TrackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches motion detection events for a Tracker.
        /// </summary>
        /// <param name="trackerId">The ID of the Tracker.</param>
        /// <returns>List of motion events.</returns>
        /// <response code="200">Returns the list of motion events.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpGet("{trackerId}")]
        ///[Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetMotionEvents(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch motion events.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var motionEvents = await _motionManager.FetchMotionEventsAsync(trackerId);
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
