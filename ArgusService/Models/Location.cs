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
        public string TrackerId { get; set; }

        /// <summary>
        /// Latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude coordinate.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Time when the location was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
