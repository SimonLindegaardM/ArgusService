using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        /// <summary>
        /// ID of the associated tracker.
        /// </summary>
        [Required(ErrorMessage = "TrackerId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Latitude coordinate.
        /// </summary>
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate.
        /// </summary>
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double Longitude { get; set; }

        /// <summary>
        /// Time when the location was recorded.
        /// </summary>
        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime Timestamp { get; set; }
    }
}
