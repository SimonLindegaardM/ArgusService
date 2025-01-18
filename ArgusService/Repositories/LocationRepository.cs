using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArgusService.Data; // Ensure this namespace matches your DbContext location
using ArgusService.Interfaces;
using ArgusService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgusService.Repositories
{
    /// <summary>
    /// Implementation of Location repository operations.
    /// </summary>
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new location entry to the database.
        /// </summary>
        public async Task AddLocationAsync(Location location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            // Verify that the associated Tracker exists
            var trackerExists = await _context.Trackers.AnyAsync(t => t.TrackerId == location.TrackerId);
            if (!trackerExists)
                throw new InvalidOperationException($"Tracker with ID '{location.TrackerId}' does not exist.");

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves location history for a specific tracker.
        /// </summary>
        public async Task<List<Location>> GetLocationHistoryAsync(string trackerId)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            return await _context.Locations
                                 .AsNoTracking()
                                 .Where(l => l.TrackerId == trackerId)
                                 .OrderByDescending(l => l.Timestamp)
                                 .ToListAsync();
        }

        /// <summary>
        /// Exports location history for a specific tracker in CSV or PDF format.
        /// Currently, only CSV is implemented.
        /// </summary>
        public async Task<byte[]> ExportLocationHistoryAsync(string trackerId, string format)
        {
            if (string.IsNullOrEmpty(trackerId))
                throw new ArgumentException("Tracker ID cannot be null or empty.");

            if (string.IsNullOrEmpty(format))
                throw new ArgumentException("Format cannot be null or empty.");

            var locations = await GetLocationHistoryAsync(trackerId);

            if (format.ToLower() == "csv")
            {
                var csv = "Latitude,Longitude,Timestamp\n" +
                          string.Join("\n", locations.Select(l => $"{l.Latitude},{l.Longitude},{l.Timestamp:O}"));
                return System.Text.Encoding.UTF8.GetBytes(csv);
            }
            else if (format.ToLower() == "pdf")
            {
                // Placeholder for PDF generation logic
                // You can use libraries like iTextSharp or PdfSharp to implement this
                throw new NotImplementedException("PDF export is not yet implemented.");
            }
            else
            {
                throw new ArgumentException("Invalid format. Allowed values are 'csv' or 'pdf'.");
            }
        }
    }
}
