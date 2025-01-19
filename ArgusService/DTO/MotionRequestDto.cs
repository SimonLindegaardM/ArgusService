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
        /// <summary>
        /// Identifier of the Tracker device detecting motion.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Indicates whether motion was detected.
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool MotionDetected { get; set; }

        /// <summary>
        /// Timestamp when the motion was detected.
        /// </summary>
        /// <example>2025-01-17T06:45:00Z</example>
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
