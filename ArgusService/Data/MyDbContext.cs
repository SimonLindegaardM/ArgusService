using Microsoft.EntityFrameworkCore;
using ArgusService.Models;

namespace ArgusService.Data
{
    /// <summary>
    /// Represents the database context for ArgusService.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of ApplicationDbContext.
        /// </summary>
        /// <param name="options">The options to configure the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for each entity
        public DbSet<User> Users { get; set; }
        public DbSet<Tracker> Trackers { get; set; }
        public DbSet<Lock> Locks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Motion> Motions { get; set; }
        public DbSet<Mqtt> MQTTs { get; set; }

        /// <summary>
        /// Configures the entity relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Lock-Tracker relationship
            modelBuilder.Entity<Lock>()
                .HasOne(l => l.Tracker)
                .WithMany(t => t.Locks)
                .HasForeignKey(l => l.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Tracker-Location relationship
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Tracker)
                .WithMany(t => t.Locations)
                .HasForeignKey(l => l.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Tracker-Motion relationship
            modelBuilder.Entity<Motion>()
                .HasOne(m => m.Tracker)
                .WithMany(t => t.Motions)
                .HasForeignKey(m => m.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Tracker-Mqtt relationship
            modelBuilder.Entity<Mqtt>()
                .HasOne(m => m.Tracker)
                .WithMany(t => t.MQTTs)
                .HasForeignKey(m => m.TrackerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Notification -> User relationship (if applicable)
            modelBuilder.Entity<Notification>()
                .HasOne<User>() // Assuming Notification has UserId but no navigation property
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
