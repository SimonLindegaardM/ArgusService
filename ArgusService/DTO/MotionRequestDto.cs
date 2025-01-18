// File: ArgusService/DTOs/MotionRequestDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for registering a new motion event.
    /// </summary>
    public class MotionRequestDto
    {
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        [Required]
        public bool MotionDetected { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
