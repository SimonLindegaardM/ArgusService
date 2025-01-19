// File: ArgusService/DTOs/LinkDeviceRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for linking a Tracker to a user account.
    /// </summary>
    public class LinkDeviceRequestDto
    {
        /// <summary>
        /// Identifier of the Tracker device to be linked.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Firebase User ID of the user to link the Tracker to.
        /// </summary>
        /// <example>UID_Alpha</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        /// <example>alpha@example.com</example>
        [Required]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 255 characters.")]
        public string Email { get; set; }
    }
}
