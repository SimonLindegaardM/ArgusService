using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    public class Tracker
    {
        /// <summary>
        /// Unique ID of the tracker.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "TrackerId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Firebase User ID of the tracker owner.
        /// </summary>
        [Required(ErrorMessage = "FirebaseUID is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Email of the tracker owner.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// MQTT username for communication.
        /// </summary>
        [Required(ErrorMessage = "MQTTUsername is required.")]
        public string MqttUsername { get; set; }

        /// <summary>
        /// MQTT password for communication.
        /// </summary>
        [Required(ErrorMessage = "MQTTPassword is required.")]
        public string MqttPassword { get; set; }

        /// <summary>
        /// URL of the MQTT broker.
        /// </summary>
        [Required(ErrorMessage = "BrokerUrl is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string BrokerUrl { get; set; }

        /// <summary>
        /// MQTT port number.
        /// </summary>
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
        public int Port { get; set; }

        /// <summary>
        /// Current lock state.
        /// </summary>
        [Required(ErrorMessage = "LockState is required.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "LockState must be between 6 and 8 characters.")]
        public string LockState { get; set; }

        /// <summary>
        /// Requested lock state.
        /// </summary>
        [StringLength(8, MinimumLength = 6, ErrorMessage = "DesiredLockState must be between 6 and 8 characters.")]
        public string DesiredLockState { get; set; }

        /// <summary>
        /// Last known location of the tracker (latitude, longitude, and timestamp).
        /// </summary>
        [NotMapped]
        public Location LastKnownLocation { get; set; }

        /// <summary>
        /// Last time the tracker was updated.
        /// </summary>
        [Required(ErrorMessage = "LastUpdated is required.")]
        public DateTime LastUpdated { get; set; }
    }
    
}
