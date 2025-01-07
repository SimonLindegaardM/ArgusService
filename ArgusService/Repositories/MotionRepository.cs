using ArgusService.Data;
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace ArgusService.Repositories
{
    public class MotionRepository : IMotion
    {
        private readonly MyDbContext _context;

        public MotionRepository(MyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a motion detection event for a tracker.
        /// </summary>
        public async Task AddMotionEventAsync(Motion motionEvent)
        {
            if (motionEvent == null)
            {
                throw new ArgumentException("Motion event cannot be null.");
            }

            await _context.Motions.AddAsync(motionEvent);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves motion events for a specific tracker.
        /// </summary>
        public async Task<List<Motion>> GetMotionEventsByTrackerIdAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
            {
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            return await _context.Motions
                .Where(m => m.TrackerId == trackerId)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }
    }
}
