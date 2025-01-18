// File: ArgusService/DTOs/AssignRoleRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for assigning a role to a user.
    /// </summary>
    public class AssignRoleRequestDto
    {
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Role must be between 1 and 50 characters.")]
        public string Role { get; set; }
    }
}
