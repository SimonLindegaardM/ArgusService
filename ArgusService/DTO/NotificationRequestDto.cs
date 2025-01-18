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
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Type must be between 1 and 50 characters.")]
        public string Type { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
