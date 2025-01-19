// File: ArgusService/DTOs/NotificationRequestDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for creating a new notification.
    /// </summary>
    public class NotificationRequestDto
    {
        /// <summary>
        /// Identifier of the Tracker device associated with the notification.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Type of the notification.
        /// </summary>
        /// <example>MotionDetected</example>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Type must be between 1 and 50 characters.")]
        public string Type { get; set; }

        /// <summary>
        /// Detailed message of the notification.
        /// </summary>
        /// <example>Motion detected at the front door.</example>
        [Required]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; }

        /// <summary>
        /// Timestamp when the notification was created.
        /// </summary>
        /// <example>2025-01-17T06:30:00Z</example>
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
