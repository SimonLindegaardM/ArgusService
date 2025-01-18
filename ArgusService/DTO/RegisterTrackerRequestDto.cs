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
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "DeviceId must be between 1 and 128 characters.")]
        public string DeviceId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "DeviceType must be between 1 and 50 characters.")]
        [RegularExpression("^(Tracker)$", ErrorMessage = "DeviceType must be 'Tracker'.")]
        public string DeviceType { get; set; } // Should be "Tracker"

        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "MqttUsername must be between 1 and 128 characters.")]
        public string MqttUsername { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "MqttPassword must be between 1 and 100 characters.")]
        public string MqttPassword { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "PSK must be between 1 and 128 characters.")]
        public string Psk { get; set; }

        [Required]
        [Url(ErrorMessage = "BrokerUrl must be a valid URL.")]
        [StringLength(255, ErrorMessage = "BrokerUrl cannot exceed 255 characters.")]
        public string BrokerUrl { get; set; }

        [Required]
        [Range(1, 65535, ErrorMessage = "Port must be between 1 and 65535.")]
        public int Port { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
