using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api")]
    public class TrackerController : ControllerBase
    {
        private readonly TrackerManager _trackerManager;

        public TrackerController(TrackerManager trackerManager)
        {
            _trackerManager = trackerManager;
        }

        /// <summary>
        /// Registers a new Tracker or Lock device.
        /// </summary>
        [HttpPost("register-device")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            try
            {
                await _trackerManager.RegisterDeviceAsync(request.DeviceId, request.DeviceType, request.AttachedTrackerId);
                return Ok(new { Message = "Device registered successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Links a Tracker or Lock to a user account.
        /// </summary>
        [HttpPost("link-device")]
        public async Task<IActionResult> LinkDevice([FromBody] LinkDeviceRequest request)
        {
            try
            {
                await _trackerManager.LinkDeviceToUserAsync(request.TrackerId, request.FirebaseUID, request.Email);
                return Ok(new { Message = "Device linked successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all devices (Trackers and Locks).
        /// </summary>
        [HttpGet("devices")]
        public async Task<IActionResult> GetAllDevices()
        {
            try
            {
                var devices = await _trackerManager.FetchAllDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the desired lock state of a tracker.
        /// </summary>
        [HttpPost("trackers/{trackerId}/lock-state")]
        public async Task<IActionResult> UpdateLockState(string trackerId, [FromBody] UpdateLockStateRequest request)
        {
            try
            {
                await _trackerManager.UpdateLockStateAsync(trackerId, request.LockState);
                return Ok(new { Message = "Lock state updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the current lock state of a tracker.
        /// </summary>
        [HttpGet("trackers/{trackerId}/lock-state")]
        public async Task<IActionResult> GetLockState(string trackerId)
        {
            try
            {
                var lockState = await _trackerManager.FetchLockStateAsync(trackerId);
                return Ok(new { LockState = lockState });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
    public class RegisterDeviceRequest
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string AttachedTrackerId { get; set; }
    }

    public class LinkDeviceRequest
    {
        public string TrackerId { get; set; }
        public string FirebaseUID { get; set; }
        public string Email { get; set; }
    }
    public class UpdateLockStateRequest
    {
        public string LockState { get; set; }
    }
}
