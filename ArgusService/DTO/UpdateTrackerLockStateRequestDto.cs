// File: ArgusService/DTOs/UpdateTrackerLockStateRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for updating the desired lock state of a Tracker.
    /// </summary>
    public class UpdateTrackerLockStateRequestDto
    {
        /// <summary>
        /// Desired lock state for the Tracker.
        /// </summary>
        /// <example>locked</example>
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "DesiredLockState must be either 'locked' or 'unlocked'.")]
        public string DesiredLockState { get; set; }
    }
}
