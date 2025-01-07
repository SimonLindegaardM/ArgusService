using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class User
    {
        [Key]
        /// <summary>
        /// Unique Firebase User ID.
        /// </summary>
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
        /// Preferences for receiving notifications.
        /// </summary>
        public string NotificationPreferences { get; set; }

        /// <summary>
        /// User's role).
        /// </summary>
        [Required(ErrorMessage = "Role is required.")]
        [StringLength(5, MinimumLength = 4, ErrorMessage = "Role must be 'user' (4 characters) or 'admin' (5 characters).")]
        [RegularExpression("^(admin|user)$", ErrorMessage = "Role must be 'admin' or 'user'.")]
        public string Role { get; set; }
    }
    public class NotificationPreferences
    {
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
    }
}
