using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique Firebase User ID.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "FirebaseUID is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// User's email address.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 100 characters.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; }

        /// <summary>
        /// Preferences for receiving notifications (as a JSON string).
        /// </summary>
        public string? NotificationPreferences { get; set; }

        /// <summary>
        /// User's role (e.g., "admin" or "user").
        /// </summary>
        [Required(ErrorMessage = "Role is required.")]
        [StringLength(5, MinimumLength = 4, ErrorMessage = "Role must be 'user' (4) or 'admin' (5).")]
        [RegularExpression("^(admin|user)$", ErrorMessage = "Role must be 'admin' or 'user'.")]
        public string Role { get; set; }

        /// <summary>
        /// Navigation property for associated notifications.
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    /// <summary>
    /// Example of a possible structured Preferences class (not bound by EF automatically).
    /// If you need to store this in the database as a separate entity, you must define a DbSet for it.
    /// </summary>
    public class NotificationPreferences
    {
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
    }
}
