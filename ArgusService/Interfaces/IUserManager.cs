using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface IUserManager
    {
        Task AssignRoleAsync(string firebaseUID, string role);
        Task<User> GetUserDetailsAsync(string firebaseUID);
        Task<bool> ValidateUserAsync(string firebaseUID);
        Task UpdateNotificationPreferencesAsync(string firebaseUID, string preferences);
        Task<List<User>> GetUsersByRoleAsync(string role);
    }
}
