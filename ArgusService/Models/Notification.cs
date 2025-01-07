using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Notification
    {
        /// <summary>
        /// Unique identifier for notifications.
        /// </summary>
        [Key]
        public string NotificationId { get; set; }

        /// <summary>
        /// Notification type (e.g., motion, lockState).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Time when the notification was generated.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Notification message.
        /// </summary>
        public string Message { get; set; }

        public string UserId { get; set; }
    }
}
