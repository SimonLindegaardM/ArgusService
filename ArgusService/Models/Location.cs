using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a geographical location associated with a Tracker.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Unique identifier for the location entry.
        /// </summary>
        [Key]
        public int LocationId { get; set; }

        /// <summary>
        /// Identifier of the Tracker associated with this location.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Latitude component of the location.
        /// </summary>
        [Required]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude component of the location.
        /// </summary>
        [Required]
        public double Longitude { get; set; }

        /// <summary>
        /// Timestamp when the location was recorded.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Navigation property to the associated Tracker.
        /// </summary>
        [ForeignKey("TrackerId")]
        public Tracker Tracker { get; set; }
    }
}
