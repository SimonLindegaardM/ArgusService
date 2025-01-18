using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs.Mqtt
{
    /// <summary>
    /// DTO for subscribing or unsubscribing to an MQTT topic.
    /// </summary>
    public class TopicRequest
    {
        /// <summary>
        /// MQTT topic to subscribe/unsubscribe.
        /// </summary>
        [Required(ErrorMessage = "Topic is required.")]
        [StringLength(256, MinimumLength = 1, ErrorMessage = "Topic must be between 1 and 256 characters.")]
        public string Topic { get; set; }
    }
}
