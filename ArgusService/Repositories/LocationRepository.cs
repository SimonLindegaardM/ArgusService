using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class LocationRepository : ILocation
    {
        private readonly MyDbContext _context;

        public LocationRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves location data (latitude, longitude) for a tracker.
        /// </summary>
        public async Task AddLocationAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves historical location data for a specific tracker.
        /// </summary>
        public async Task<List<Location>> GetLocationHistoryAsync(string trackerId)
        {
            return await _context.Locations
                .Where(l => l.TrackerId == trackerId)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}
