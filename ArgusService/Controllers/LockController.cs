using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api")]
    public class LockController : ControllerBase
    {
        private readonly LockManager _lockManager;

        public LockController(LockManager lockManager)
        {
            _lockManager = lockManager;
        }

        /// <summary>
        /// Registers a new Lock device.
        /// </summary>
        [HttpPost("locks")]
        public async Task<IActionResult> RegisterLock([FromBody] RegisterLockRequest request)
        {
            try
            {
                await _lockManager.RegisterLockToTrackerAsync(request.LockId, request.TrackerId);
                return Ok(new { Message = "Lock registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all locks attached to a tracker.
        /// </summary>
        [HttpGet("trackers/{trackerId}/locks")]
        public async Task<IActionResult> GetLocksByTrackerId(string trackerId)
        {
            try
            {
                var locks = await _lockManager.FetchLocksByTrackerIdAsync(trackerId);
                return Ok(locks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class RegisterLockRequest
    {
        public string LockId { get; set; }
        public string TrackerId { get; set; }
    }
}
