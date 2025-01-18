using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArgusService.Interfaces;
using ArgusService.Models;
using ArgusService.Repositories; // Typically you'd reference interfaces only, but here we have direct usage.

namespace ArgusService.Managers
{
    /// <summary>
    /// UserManager implements IUserManager and uses an IUserRepository for data access.
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;

        // Note: Better to depend on IUserRepository, not the concrete UserRepository
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Assigns a role to a user (admin-only action).
        /// </summary>
        public async Task AssignRoleAsync(string firebaseUID, string role)
        {
            if (role != "admin" && role != "user")
            {
                throw new ArgumentException("Invalid role. Allowed roles are 'admin' or 'user'.");
            }

            if (!await ValidateUserAsync(firebaseUID))
            {
                throw new Exception($"User with FirebaseUID {firebaseUID} does not exist.");
            }

            await _userRepository.UpdateUserRoleAsync(firebaseUID, role);
        }

        /// <summary>
        /// Fetches user details using their Firebase UID.
        /// </summary>
        public async Task<User> GetUserDetailsAsync(string firebaseUID)
        {
            if (!await ValidateUserAsync(firebaseUID))
            {
                throw new Exception($"User with FirebaseUID {firebaseUID} does not exist.");
            }

            return await _userRepository.GetUserDetailsAsync(firebaseUID);
        }

        /// <summary>
        /// Validates that a user exists in the database by checking their role.
        /// </summary>
        public async Task<bool> ValidateUserAsync(string firebaseUID)
        {
            var role = await _userRepository.GetUserRoleAsync(firebaseUID);
            return role != null;
        }

        /// <summary>
        /// Updates a user's notification preferences.
        /// </summary>
        public async Task UpdateNotificationPreferencesAsync(string firebaseUID, string preferences)
        {
            if (!await ValidateUserAsync(firebaseUID))
            {
                throw new Exception($"User with FirebaseUID {firebaseUID} does not exist.");
            }

            var user = await _userRepository.GetUserDetailsAsync(firebaseUID);
            if (user != null)
            {
                user.NotificationPreferences = preferences;
                await _userRepository.UpdateUserAsync(user);
            }
        }

        /// <summary>
        /// Retrieves all users with a specific role.
        /// </summary>
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            if (role != "admin" && role != "user")
            {
                throw new ArgumentException("Invalid role. Allowed roles are 'admin' or 'user'.");
            }

            return await _userRepository.GetUsersByRoleAsync(role);
        }
    }
}
