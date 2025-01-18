using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a motion event detected by a Tracker.
    /// </summary>
    public class Motion
    {
        /// <summary>
        /// Unique identifier for the motion event.
        /// </summary>
        [Key]
        public int MotionId { get; set; }

        /// <summary>
        /// Identifier of the Tracker associated with this motion event.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Indicates whether motion was detected.
        /// </summary>
        [Required]
        public bool MotionDetected { get; set; }

        /// <summary>
        /// Timestamp when the motion was detected.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Navigation property to the associated Tracker.
        /// </summary>
        [ForeignKey(nameof(TrackerId))]
        public Tracker Tracker { get; set; }
    }
}
