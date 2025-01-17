﻿using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class UserRepository : IUser
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves the role assigned to a user by their Firebase UID.
        /// </summary>
        public async Task<string> GetUserRoleAsync(string firebaseUID)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUID);
            return user?.Role;
        }

        /// <summary>
        /// Updates the role of a user (e.g., "user", "admin").
        /// </summary>
        public async Task UpdateUserRoleAsync(string firebaseUID, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUID);
            if (user != null)
            {
                user.Role = role;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Fetches user details using their Firebase UID.
        /// </summary>
        public async Task<User> GetUserDetailsAsync(string firebaseUID)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUID);
        }

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == user.FirebaseUID);
            if (existingUser != null)
            {
                existingUser.Email = user.Email;
                existingUser.NotificationPreferences = user.NotificationPreferences;
                existingUser.Role = user.Role;
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Creates a new user with a default role.
        /// </summary>
        public async Task CreateUserAsync(User user)
        {
            if (user != null)
            {
                user.Role ??= "user"; // Default role as "user"
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users.Where(u => u.Role == role).ToListAsync();
        }
    }
}
