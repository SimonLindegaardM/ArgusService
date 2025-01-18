using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents the MQTT configuration and status for a Tracker.
    /// </summary>
    public class Mqtt
    {
        /// <summary>
        /// Unique ID for the MQTT configuration.
        /// </summary>
        [Key]
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "MqttId must be between 1 and 128 characters.")]
        public string MqttId { get; set; }

        /// <summary>
        /// ID of the associated tracker.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Topic for MQTT communication.
        /// </summary>
        [Required]
        [StringLength(256, ErrorMessage = "MqttTopic cannot exceed 256 characters.")]
        public string MqttTopic { get; set; }

        /// <summary>
        /// Quality of Service level.
        /// </summary>
        [Required]
        [Range(0, 2, ErrorMessage = "QoS must be between 0 and 2.")]
        public int Qos { get; set; }

        /// <summary>
        /// Time when the last message was sent/received.
        /// </summary>
        [Required]
        public DateTime LastMessageTimestamp { get; set; }

        /// <summary>
        /// Retain flag for MQTT messages.
        /// </summary>
        [Required]
        public bool Retain { get; set; }

        /// <summary>
        /// MQTT connection status.
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        /// <summary>
        /// Navigation property to the associated Tracker.
        /// </summary>
        [ForeignKey(nameof(TrackerId))]
        public Tracker Tracker { get; set; }
    }
}
