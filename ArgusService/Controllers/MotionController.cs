using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api")]
    public class MotionController : ControllerBase
    {
        private readonly MotionManager _motionManager;

        public MotionController(MotionManager motionManager)
        {
            _motionManager = motionManager;
        }

        /// <summary>
        /// Logs a motion detection event for a tracker.
        /// </summary>
        [HttpPost("motions")]
        public async Task<IActionResult> LogMotionEvent([FromBody] LogMotionEventRequest request)
        {
            try
            {
                await _motionManager.LogMotionEventAsync(request.TrackerId, request.MotionDetected);
                return Ok(new { Message = "Motion event logged successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches motion detection events for a tracker.
        /// </summary>
        [HttpGet("motions/{trackerId}")]
        public async Task<IActionResult> GetMotionEvents(string trackerId)
        {
            try
            {
                var motionEvents = await _motionManager.FetchMotionEventsAsync(trackerId);
                return Ok(motionEvents);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class LogMotionEventRequest
    {
        public string TrackerId { get; set; }
        public bool MotionDetected { get; set; }
    }
}
