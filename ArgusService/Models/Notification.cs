using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Notification
    {
        /// <summary>
        /// Unique identifier for notifications.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "NotificationId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "NotificationId must be between 1 and 128 characters.")]
        public string NotificationId { get; set; }

        /// <summary>
        /// Notification type.
        /// </summary>
        [Required(ErrorMessage = "Type is required.")]
        [RegularExpression("^(motion|lockState)$", ErrorMessage = "Type must be 'motion' or 'lockState'.")]
        public string Type { get; set; }

        /// <summary>
        /// Time when the notification was generated.
        /// </summary>
        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Notification message.
        /// </summary>
        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 500 characters.")]
        public string Message { get; set; }

        public string UserId { get; set; }
    }
}
