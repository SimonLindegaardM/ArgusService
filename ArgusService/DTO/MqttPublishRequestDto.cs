// File: ArgusService/DTOs/MqttPublishRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for publishing an MQTT message.
    /// </summary>
    public class MqttPublishRequestDto
    {
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "TopicType must be between 1 and 50 characters.")]
        public string TopicType { get; set; }

        [Required]
        public object Payload { get; set; }
    }
}
