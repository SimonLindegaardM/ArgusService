using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Lock
    {
        /// <summary>
        /// Unique identifier for the lock.
        /// </summary>
        [Key]
        [Required(ErrorMessage = "DeviceId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "DeviceId must be between 1 and 128 characters.")]
        public string DeviceId { get; set; }

        /// <summary>
        /// Firebase User ID of the lock owner.
        /// </summary>
        [Required(ErrorMessage = "FirebaseUID is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 128 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Email of the lock owner.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        /// <summary>
        /// ID of the tracker linked to the lock.
        /// </summary>
        [Required(ErrorMessage = "AttachedTrackerId is required.")]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "AttachedTrackerId must be between 1 and 128 characters.")]
        public string AttachedTrackerId { get; set; }

        /// <summary>
        /// Current lock state.
        /// </summary>
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "Type must be 'locked' or 'unlocked'.")]
        public string Status { get; set; }

        /// <summary>
        /// Last update timestamp for the lock state.
        /// </summary>
        [Required(ErrorMessage = "LastUpdated is required.")]
        public DateTime LastUpdated { get; set; }
    }
}
