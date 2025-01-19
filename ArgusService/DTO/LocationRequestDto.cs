// File: ArgusService/DTOs/LocationRequestDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for registering a new location.
    /// </summary>
    public class LocationRequestDto
    {
        /// <summary>
        /// Identifier of the Tracker device providing the location.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Latitude component of the location.
        /// </summary>
        /// <example>37.7749</example>
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude component of the location.
        /// </summary>
        /// <example>-122.4194</example>
        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }

        /// <summary>
        /// Timestamp when the location was recorded.
        /// </summary>
        /// <example>2025-01-17T07:00:00Z</example>
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
