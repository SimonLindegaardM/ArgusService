// File: ArgusService/DTOs/LinkDeviceRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for linking a Tracker to a user account.
    /// </summary>
    public class LinkDeviceRequestDto
    {
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 255 characters.")]
        public string Email { get; set; }
    }
}
