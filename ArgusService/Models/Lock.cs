using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArgusService.Models
{
    /// <summary>
    /// Represents a physical lock device linked to a Tracker.
    /// </summary>
    public class Lock
    {
        /// <summary>
        /// Unique identifier for the lock device (e.g., "Lock123").
        /// </summary>
        [Key]
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "LockId must be between 1 and 100 characters.")]
        public string LockId { get; set; }

        /// <summary>
        /// ID of the tracker this lock is attached to (e.g., "Tracker999").
        /// Must exist in the Trackers table.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1, ErrorMessage = "TrackerId must be between 1 and 128 characters.")]
        public string TrackerId { get; set; }

        /// <summary>
        /// Firebase User ID of the lock owner.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "FirebaseUID must be between 1 and 100 characters.")]
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Owner's email address (for quick reference).
        /// </summary>
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 255 characters.")]
        public string? Email { get; set; }

        /// <summary>
        /// Current lock status ("locked" or "unlocked").
        /// </summary>
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "Status must be either 'locked' or 'unlocked'.")]
        public string Status { get; set; }
        
        /// <summary>
        /// Last time the lock status was updated.
        /// </summary>
        [Required]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Navigation property to the associated Tracker.
        /// </summary>
        [ForeignKey(nameof(TrackerId))]
        public Tracker Tracker { get; set; }
    }
}
