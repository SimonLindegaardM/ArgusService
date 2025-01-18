using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Models;

namespace ArgusService.Interfaces
{
    public interface IUserRepository
    {
        Task<string> GetUserRoleAsync(string firebaseUID);
        Task UpdateUserRoleAsync(string firebaseUID, string role);
        Task<User> GetUserDetailsAsync(string firebaseUID);
        Task UpdateUserAsync(User user);
        Task CreateUserAsync(User user);
        Task<List<User>> GetUsersByRoleAsync(string role);
    }
}
