using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Mqtt
    {
        /// <summary>
        /// Unique ID for the MQTT configuration.
        /// </summary>
        [Key]
        public string MqttId { get; set; }

        /// <summary>
        /// ID of the associated tracker.
        /// </summary>
        public string TrackerId { get; set; }

        /// <summary>
        /// Topic for MQTT communication.
        /// </summary>
        public string MqttTopic { get; set; }

        /// <summary>
        /// Quality of Service level.
        /// </summary>
        public int Qos { get; set; }

        /// <summary>
        /// Time when the last message was sent/received.
        /// </summary>
        public DateTime LastMessageTimestamp { get; set; }

        /// <summary>
        /// Retain flag for MQTT messages.
        /// </summary>
        public bool Retain { get; set; }

        /// <summary>
        /// MQTT connection status.
        /// </summary>
        public string Status { get; set; }
    }
}
