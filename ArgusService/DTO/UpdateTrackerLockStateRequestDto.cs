// File: ArgusService/DTOs/UpdateTrackerLockStateRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for updating the desired lock state of a Tracker.
    /// </summary>
    public class UpdateTrackerLockStateRequestDto
    {
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "DesiredLockState must be either 'locked' or 'unlocked'.")]
        public string DesiredLockState { get; set; }
    }
}
