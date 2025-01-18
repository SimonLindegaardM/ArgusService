// File: ArgusService/Controllers/UserController.cs

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArgusService.Interfaces;   // IUserManager interface
using ArgusService.DTOs;         // AssignRoleRequestDto
using Microsoft.Extensions.Logging;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserManager userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Assigns a role to a user (admin-only).
        /// </summary>
        [HttpPost("assign-role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid AssignRoleRequestDto received.");
                return BadRequest(ModelState);
            }

            try
            {
                await _userManager.AssignRoleAsync(request.FirebaseUID, request.Role);
                _logger.LogInformation("Role '{Role}' assigned to user '{FirebaseUID}'.", request.Role, request.FirebaseUID);
                return Ok(new { Message = "Role assigned successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role to user '{FirebaseUID}'.", request.FirebaseUID);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches user details by Firebase UID.
        /// </summary>
        [HttpGet("{firebaseUID}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserDetails(string firebaseUID)
        {
            try
            {
                var user = await _userManager.GetUserDetailsAsync(firebaseUID);
                if (user == null)
                {
                    _logger.LogWarning("User with FirebaseUID '{FirebaseUID}' not found.", firebaseUID);
                    return NotFound(new { Message = "User not found." });
                }

                _logger.LogInformation("Fetched details for user '{FirebaseUID}'.", firebaseUID);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details for user '{FirebaseUID}'.", firebaseUID);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Fetches users based on their role (admin-only).
        /// </summary>
        [HttpGet("by-role/{role}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Role parameter is required to fetch users by role.");
                return BadRequest(new { Message = "Role parameter is required." });
            }

            try
            {
                var users = await _userManager.GetUsersByRoleAsync(role);
                _logger.LogInformation("Fetched users with role '{Role}'.", role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users by role '{Role}'.", role);
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
