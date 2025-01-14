using ArgusService.Interfaces;
using ArgusService.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArgusService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UserController(UserManager userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Assigns a role to a user (admin-only).
        /// </summary>
        [HttpPost("assign-role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            if (!User.IsInRole("admin"))
            {
                return Forbid("Only admins can assign roles.");
            }

            try
            {
                await _userManager.AssignRoleAsync(request.FirebaseUID, request.Role);
                return Ok(new { Message = "Role assigned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        /// <summary>
        /// Fetches user details by Firebase UID.
        /// </summary>
        [HttpGet("{firebaseUID}")]
        public async Task<IActionResult> GetUserDetails(string firebaseUID)
        {
            try
            {
                var user = await _userManager.GetUserDetailsAsync(firebaseUID);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
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
            if (!User.IsInRole("admin"))
            {
                return Forbid("Only admins can view users by role.");
            }

            try
            {
                var users = await _userManager.GetUsersByRoleAsync(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class AssignRoleRequest
    {
        public string FirebaseUID { get; set; }
        public string Role { get; set; }
    }
}
