using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ArgusService.Data;

namespace ArgusService.Repositories
{
    /// <summary>
    /// Repository for Motion-related data operations.
    /// </summary>
    public class MotionRepository : IMotionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MotionRepository> _logger;

        /// <summary>
        /// Initializes a new instance of MotionRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public MotionRepository(ApplicationDbContext context, ILogger<MotionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new motion detection event.
        /// </summary>
        public async Task AddMotionEventAsync(Motion motionEvent)
        {
            if (motionEvent == null)
            {
                _logger.LogWarning("Attempted to add a null motion event.");
                throw new ArgumentException("Motion event cannot be null.");
            }

            _logger.LogInformation("Adding motion event for Tracker '{TrackerId}' detected at {Timestamp}.", motionEvent.TrackerId, motionEvent.Timestamp);

            // Optionally, verify that the Tracker exists
            bool trackerExists = await _context.Trackers.AnyAsync(t => t.TrackerId == motionEvent.TrackerId);
            if (!trackerExists)
            {
                _logger.LogWarning("Tracker with ID '{TrackerId}' does not exist. Cannot add motion event.", motionEvent.TrackerId);
                throw new InvalidOperationException($"Tracker with ID '{motionEvent.TrackerId}' does not exist.");
            }

            await _context.Motions.AddAsync(motionEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Motion event for Tracker '{TrackerId}' added successfully.", motionEvent.TrackerId);
        }

        /// <summary>
        /// Retrieves all motion events for a specific Tracker.
        /// </summary>
        public async Task<List<Motion>> GetMotionEventsByTrackerIdAsync(string trackerId)
        {
            _logger.LogInformation("Fetching motion events for Tracker '{TrackerId}'.", trackerId);

            if (string.IsNullOrEmpty(trackerId))
            {
                _logger.LogWarning("Tracker ID is null or empty. Cannot fetch motion events.");
                throw new ArgumentException("Tracker ID cannot be null or empty.");
            }

            var motionEvents = await _context.Motions
                                 .AsNoTracking()
                                 .Where(m => m.TrackerId == trackerId)
                                 .OrderByDescending(m => m.Timestamp)
                                 .ToListAsync();

            _logger.LogInformation("Fetched {Count} motion events for Tracker '{TrackerId}'.", motionEvents.Count, trackerId);
            return motionEvents;
        }
    }
}
