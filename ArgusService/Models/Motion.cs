using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Motion
    {
        /// <summary>
        /// ID of the associated tracker.
        /// </summary>
        [Required(ErrorMessage = "TrackerId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Indicates whether motion has been detected.
        /// </summary>
        [Required(ErrorMessage = "MotionDetected is required.")]
        public bool MotionDetected { get; set; }

        /// <summary>
        /// Timestamp when the motion was detected.
        /// </summary>
        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime Timestamp { get; set; }
    }
}
