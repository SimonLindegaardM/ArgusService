using System.ComponentModel.DataAnnotations;

namespace ArgusService.Models
{
    public class Lock
    {
        /// <summary>
        /// Unique identifier for the lock.
        /// </summary>
        [Key]
        public string DeviceId { get; set; }

        /// <summary>
        /// Firebase User ID of the lock owner.
        /// </summary>
        public string FirebaseUID { get; set; }

        /// <summary>
        /// Email of the lock owner.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ID of the tracker linked to the lock.
        /// </summary>
        public string AttachedTrackerId { get; set; }

        /// <summary>
        /// Current lock state.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Last update timestamp for lock state.
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
