using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a Tracker device in the system.
    /// </summary>
    public class Tracker
    {
        /// <summary>
        /// Unique identifier for the tracker device.
        /// </summary>
        [Key]
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// The Firebase User ID of the tracker owner.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Owner's email address for reference.
        /// </summary>
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 100 characters.")]
        public string? Email { get; set; }

        /// <summary>
        /// MQTT username for device communication.
        /// </summary>
        [StringLength(100, MinimumLength = 1, ErrorMessage = "MqttUsername must be between 1 and 100 characters.")]
        public string? MqttUsername { get; set; }

        /// <summary>
        /// MQTT password for device communication.
        /// </summary>
        [StringLength(100, MinimumLength = 1, ErrorMessage = "MqttPassword must be between 1 and 100 characters.")]
        public string? MqttPassword { get; set; }

        /// <summary>
        /// MQTT broker URL the tracker connects to.
        /// </summary>
        [Url(ErrorMessage = "BrokerUrl must be a valid URL.")]
        [StringLength(255, ErrorMessage = "BrokerUrl cannot exceed 255 characters.")]
        public string? BrokerUrl { get; set; }

        /// <summary>
        /// MQTT port number.
        /// </summary>
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
        public int Port { get; set; }

        /// <summary>
        /// Current lock state (e.g., "locked" or "unlocked").
        /// This is the tracker's notion of whether it's locked or not.
        /// </summary>
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "LockState must be either 'locked' or 'unlocked'.")]
        public string LockState { get; set; }

        /// <summary>
        /// Requested lock state (e.g., "locked" or "unlocked").
        /// Could differ from actual LockState if the device is offline or pending.
        /// </summary>
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "DesiredLockState must be either 'locked' or 'unlocked'.")]
        public string DesiredLockState { get; set; }

        /// <summary>
        /// Last known location data (could be lat/long, or JSON).
        /// </summary>
        public string? LastKnownLocation { get; set; }

        /// <summary>
        /// Last time the tracker was updated (telemetry, state, etc.).
        /// </summary>
        [Required]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Collection of locks associated with this tracker.
        /// </summary>
        public ICollection<Lock> Locks { get; set; } = new List<Lock>();

        /// <summary>
        /// Collection of motions associated with this tracker.
        /// </summary>
        public ICollection<Motion> Motions { get; set; } = new List<Motion>();

        /// <summary>
        /// Collection of MQTT configurations associated with this tracker.
        /// </summary>
        public ICollection<Mqtt> MQTTs { get; set; } = new List<Mqtt>();

        /// <summary>
        /// Collection of locations associated with this tracker.
        /// </summary>
        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
}
