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
        public string TrackerId { get; set; }

        /// <summary>
        /// Firebase User ID of the tracker owner.
        /// </summary>
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Email of the tracker owner.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// MQTT username for communication.
        /// </summary>
        public string MqttUsername { get; set; }

        /// <summary>
        /// MQTT password for communication.
        /// </summary>
        public string MqttPassword { get; set; }

        /// <summary>
        /// URL of the MQTT broker.
        /// </summary>
        public string BrokerUrl { get; set; }

        /// <summary>
        /// MQTT port number.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Current lock state.
        /// </summary>
        public string LockState { get; set; }

        /// <summary>
        /// Requested lock state.
        /// </summary>
        public string DesiredLockState { get; set; }

        /// <summary>
        /// Last known location of the tracker (latitude, longitude, and timestamp).
        /// </summary>
        [NotMapped]
        public Location LastKnownLocation { get; set; }

        /// <summary>
        /// Last time the tracker was updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
    
}
