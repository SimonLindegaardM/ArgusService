// File: ArgusService/DTOs/RegisterLockRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for registering a new Lock device.
    /// </summary>
    public class RegisterLockRequestDto
    {
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "LockId must be between 1 and 128 characters.")]
        public string LockId { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }
    }
}
