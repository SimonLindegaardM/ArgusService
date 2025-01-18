// File: ArgusService/Controllers/TrackerController.cs

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArgusService.Controllers
{
    /// <summary>
    /// Controller for managing Tracker devices.
    /// </summary>
    [ApiController]
    [Route("api/trackers")]
    public class TrackerController : ControllerBase
    {
        private readonly ITrackerManager _trackerManager;
        private readonly ILogger<TrackerController> _logger;

        public TrackerController(ITrackerManager trackerManager, ILogger<TrackerController> logger)
        {
            _trackerManager = trackerManager;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new Tracker device.
        /// Example body:
        /// {
        ///   "deviceId": "Tracker001",
        ///   "deviceType": "Tracker",
        ///   "mqttUsername": "mqttUser",
        ///   "mqttPassword": "mqttPass",
        ///   "psk": "securePSK",
        ///   "brokerUrl": "http://broker.example.com",
        ///   "port": 1883,
        ///   "createdAt": "2025-01-17T06:00:00Z"
        /// }
        /// </summary>
        [HttpPost]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterTracker([FromBody] RegisterTrackerRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid RegisterTrackerRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _trackerManager.RegisterDeviceAsync(request.DeviceId, request.DeviceType, null);
                _logger.LogInformation("Tracker '{DeviceId}' registered successfully.", request.DeviceId);
                return CreatedAtAction(nameof(RegisterTracker), new { request.DeviceId }, new { Message = "Tracker registered successfully." });
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "Invalid operation during tracker registration.");
                return BadRequest(new { Message = invOpEx.Message });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Argument exception during tracker registration.");
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during tracker registration.");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Links a Tracker to a user account.
        /// Example body:
        /// {
        ///   "trackerId": "Tracker001",
        ///   "firebaseUID": "User123",
        ///   "email": "user@example.com"
        /// }
        /// </summary>
        [HttpPost("link-device")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> LinkDevice([FromBody] LinkDeviceRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid LinkDeviceRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _trackerManager.LinkDeviceToUserAsync(request.TrackerId, request.FirebaseUID, request.Email);
                _logger.LogInformation("Tracker '{TrackerId}' linked to user '{FirebaseUID}'.", request.TrackerId, request.FirebaseUID);
                return Ok(new { Message = "Tracker linked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking tracker to user.");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches all Trackers in the system.
        /// </summary>
        [HttpGet]
        // [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetAllTrackers()
        {
            try
            {
                var trackers = await _trackerManager.GetAllDevicesAsync();
                _logger.LogInformation("Fetched all trackers.");
                return Ok(trackers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all trackers.");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Updates the desired lock state of a Tracker.
        /// Example body:
        /// { "desiredLockState": "locked" }
        /// </summary>
        [HttpPost("{trackerId}/lock-state")]
        // [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> UpdateLockState(string trackerId, [FromBody] UpdateTrackerLockStateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid UpdateTrackerLockStateRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _trackerManager.UpdateLockStateAsync(trackerId, request.DesiredLockState);
                _logger.LogInformation("Tracker '{TrackerId}' lock state updated to '{DesiredLockState}'.", trackerId, request.DesiredLockState);
                return Ok(new { Message = $"Tracker lock state updated to {request.DesiredLockState}." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while updating lock state for Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lock state for Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Retrieves the current lock state of a Tracker.
        /// </summary>
        [HttpGet("{trackerId}/lock-state")]
        // [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetLockState(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch lock state.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var lockState = await _trackerManager.FetchLockStateAsync(trackerId);
                if (lockState == null)
                {
                    _logger.LogWarning("Lock state for Tracker '{TrackerId}' not found.", trackerId);
                    return NotFound(new { Message = "Tracker not found or LockState not set." });
                }

                _logger.LogInformation("Fetched lock state for Tracker '{TrackerId}': {LockState}.", trackerId, lockState);
                return Ok(new { LockState = lockState });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lock state for Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Deletes a Tracker and all associated data.
        /// </summary>
        [HttpDelete("{trackerId}")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTracker(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to delete.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                await _trackerManager.DeleteTrackerAsync(trackerId);
                _logger.LogInformation("Tracker '{TrackerId}' deleted successfully.", trackerId);
                return Ok(new { Message = $"Tracker with ID '{trackerId}' deleted successfully." });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Invalid argument while deleting Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = argEx.Message });
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogWarning(invOpEx, "Tracker '{TrackerId}' not found for deletion.", trackerId);
                return NotFound(new { Message = invOpEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Tracker '{TrackerId}'.", trackerId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
