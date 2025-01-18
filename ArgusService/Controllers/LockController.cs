// File: ArgusService/Controllers/LockController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.DTOs;
using ArgusService.Interfaces;   // ILockManager interface
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AutoMapper; // Add this
using ArgusService.Models; // Ensure this is included for the Lock model

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/locks")]
    public class LockController : ControllerBase
    {
        private readonly ILockManager _lockManager;
        private readonly ILogger<LockController> _logger;
        private readonly IMapper _mapper; // Inject IMapper

        public LockController(ILockManager lockManager, ILogger<LockController> logger, IMapper mapper)
        {
            _lockManager = lockManager;
            _logger = logger;
            _mapper = mapper; // Assign IMapper
        }

        /// <summary>
        /// Registers a new Lock device to a Tracker.
        /// Example body:
        /// {
        ///   "lockId": "Lock123",
        ///   "trackerId": "Tracker999"
        /// }
        /// </summary>
        [HttpPost]
        ///[Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterLock([FromBody] RegisterLockRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid RegisterLockRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Model
                var lockEntity = _mapper.Map<Lock>(request);

                await _lockManager.RegisterLockToTrackerAsync(lockEntity.LockId, lockEntity.TrackerId);
                _logger.LogInformation("Lock '{LockId}' registered to Tracker '{TrackerId}'.", lockEntity.LockId, lockEntity.TrackerId);
                return Ok(new { Message = "Lock registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering Lock '{LockId}' to Tracker '{TrackerId}'.", request.LockId, request.TrackerId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all Locks attached to a specific Tracker.
        /// GET /api/locks/trackers/{trackerId}/locks
        /// </summary>
        [HttpGet("trackers/{trackerId}/locks")]
        //[Authorize(Roles = "admin,user")] // Uncomment if needed
        public async Task<IActionResult> GetLocksByTrackerId(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is required to fetch Locks.");
                return BadRequest(new { Message = "Tracker ID is required." });
            }

            try
            {
                var locks = await _lockManager.FetchLocksByTrackerIdAsync(trackerId);
                _logger.LogInformation("Fetched Locks for Tracker '{TrackerId}'.", trackerId);
                return Ok(locks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Locks for Tracker '{TrackerId}'.", trackerId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the lock state of a specific Lock.
        /// Example body:
        /// { "lockState": "locked" }
        /// </summary>
        [HttpPost("{lockId}/lock-state")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> UpdateLockState(string lockId, [FromBody] UpdateLockLockStateRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid UpdateLockLockStateRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _lockManager.UpdateLockStateAsync(lockId, request.LockState);
                _logger.LogInformation("Lock '{LockId}' state updated to '{LockState}'.", lockId, request.LockState);
                return Ok(new { Message = $"Lock state updated to {request.LockState}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Lock state for Lock '{LockId}'.", lockId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches all Locks in the system.
        /// GET /api/locks
        /// </summary>
        [HttpGet]
        ///[Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllLocks()
        {
            try
            {
                var allLocks = await _lockManager.GetAllLocksAsync();
                _logger.LogInformation("Fetched all Locks.");
                return Ok(allLocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all Locks.");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Fetches details of a specific Lock.
        /// GET /api/locks/{lockId}
        /// </summary>
        [HttpGet("{lockId}")]
        ///[Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetLockById(string lockId)
        {
            if (string.IsNullOrEmpty(lockId))
            {
                _logger.LogWarning("Lock ID is required to fetch details.");
                return BadRequest(new { Message = "Lock ID is required." });
            }

            try
            {
                var lockDetails = await _lockManager.GetLockByIdAsync(lockId);
                if (lockDetails == null)
                {
                    _logger.LogWarning("Lock '{LockId}' not found.", lockId);
                    return NotFound(new { Message = "Lock not found." });
                }

                _logger.LogInformation("Fetched details for Lock '{LockId}'.", lockId);
                return Ok(lockDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details for Lock '{LockId}'.", lockId);
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}
