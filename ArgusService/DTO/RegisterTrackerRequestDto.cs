// File: ArgusService/DTOs/RegisterTrackerRequestDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for registering a new Tracker device.
    /// </summary>
    public class RegisterTrackerRequestDto
    {
        /// <summary>
        /// Unique identifier for the Tracker device.
        /// </summary>
        /// <example>Tracker001</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "DeviceId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Type of the device. Must be 'Tracker'.
        /// </summary>
        /// <example>Tracker</example>
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "DeviceType must be between 1 and 50 characters.")]
        [RegularExpression("^(Tracker)$", ErrorMessage = "DeviceType must be 'Tracker'.")]
        public string DeviceType { get; set; }

        /// <summary>
        /// MQTT username for device communication.
        /// </summary>
        /// <example>mqttUser</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "MqttUsername must be between 1 and 128 characters.")]
        public string MqttUsername { get; set; }

        /// <summary>
        /// MQTT password for device communication.
        /// </summary>
        /// <example>mqttPass</example>
       // [Required]
        //[StringLength(100, MinimumLength = 1, ErrorMessage = "MqttPassword must be between 1 and 100 characters.")]
     //   public string MqttPassword { get; set; }

        /// <summary>
        /// Pre-Shared Key (PSK) for secure communication.
        /// </summary>
        /// <example>securePSK</example>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "PSK must be between 1 and 128 characters.")]
        public string Psk { get; set; }

        /// <summary>
        /// MQTT broker URL the Tracker connects to.
        /// </summary>
        /// <example>http://broker.example.com</example>
        [Required]
        //[Url(ErrorMessage = "BrokerUrl must be a valid URL.")]
        [StringLength(255, ErrorMessage = "BrokerUrl cannot exceed 255 characters.")]
        public string BrokerUrl { get; set; }

        /// <summary>
        /// MQTT port number.
        /// </summary>
        /// <example>1883</example>
        [Required]
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
        public int Port { get; set; }

        /// <summary>
        /// Timestamp when the Tracker was created.
        /// </summary>
        /// <example>2025-01-17T06:00:00Z</example>
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
