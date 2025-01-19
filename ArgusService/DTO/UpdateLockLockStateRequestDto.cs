// File: ArgusService/DTOs/UpdateLockLockStateRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace ArgusService.DTOs
{
    /// <summary>
    /// DTO for updating the lock state of a Lock.
    /// </summary>
    public class UpdateLockLockStateRequestDto
    {
        /// <summary>
        /// New lock state for the Lock.
        /// </summary>
        /// <example>unlocked</example>
        [Required]
        [RegularExpression("^(locked|unlocked)$", ErrorMessage = "LockState must be either 'locked' or 'unlocked'.")]
        public string LockState { get; set; }
    }
}
