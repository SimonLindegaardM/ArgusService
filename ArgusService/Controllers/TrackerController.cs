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
        /// </summary>
        /// <param name="request">The Tracker registration details.</param>
        /// <returns>A newly created Tracker.</returns>
        /// <response code="201">Tracker registered successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterTracker([FromBody] RegisterTrackerRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid RegisterTrackerRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _trackerManager.RegisterDeviceAsync(request.TrackerId, request.DeviceType, null);
                _logger.LogInformation("Tracker '{DeviceId}' registered successfully.", request.TrackerId);
                return CreatedAtAction(nameof(RegisterTracker), new { request.TrackerId }, new { Message = "Tracker registered successfully." });
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
        /// </summary>
        /// <param name="request">Details for linking the Tracker to a user.</param>
        /// <returns>Result of the linking operation.</returns>
        /// <response code="200">Tracker linked successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost("link")]
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
        /// <returns>List of all Trackers.</returns>
        /// <response code="200">Returns the list of Trackers.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpGet("devices")]
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
        /// </summary>
        /// <param name="trackerId">The ID of the Tracker to update.</param>
        /// <param name="request">The desired lock state.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Lock state updated successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpPost("{trackerId}/lock-state")]
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
        /// <param name="trackerId">The ID of the Tracker.</param>
        /// <returns>The current lock state.</returns>
        /// <response code="200">Returns the lock state.</response>
        /// <response code="404">If the Tracker is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpGet("{trackerId}/lock-state")]
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
        /// <param name="trackerId">The ID of the Tracker to delete.</param>
        /// <returns>Result of the deletion operation.</returns>
        /// <response code="200">Tracker deleted successfully.</response>
        /// <response code="400">If the input is invalid.</response>
        /// <response code="404">If the Tracker is not found.</response>
        /// <response code="500">If an unexpected error occurs.</response>
        [HttpDelete("{trackerId}")]
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
