// File: ArgusService/DTOs/RegisterLockRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for registering a new Lock device.
    /// </summary>
    public class RegisterLockRequestDto
    {
        /// <summary>
        /// Unique identifier for the Lock device.
        /// </summary>
        /// <example>Lock001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "LockId must be between 1 and 128 characters.")]
        public string LockId { get; set; }

        /// <summary>
        /// Identifier of the Tracker device to which this Lock is attached.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }
    }
}
