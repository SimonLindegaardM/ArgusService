using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a notification associated with a user (or device).
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Unique identifier for the notification.
        /// </summary>
        [Key]
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "NotificationId must be between 1 and 128 characters.")]
        public string NotificationId { get; set; }

        /// <summary>
        /// Notification type (e.g., "motion", "lockState").
        /// </summary>
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
        public string? Type { get; set; }

        /// <summary>
        /// Time when the notification was generated.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Notification message content.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The user ID associated with this notification.
        /// </summary>
        [StringLength(128, ErrorMessage = "UserId cannot exceed 128 characters.")]
        public string? UserId { get; set; }

        /// <summary>
        /// Navigation property to the associated User.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
