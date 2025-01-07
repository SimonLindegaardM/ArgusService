using ArgusService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArgusService.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tracker> Trackers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Motion> Motions { get; set; }
        public DbSet<Lock> Locks { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Mqtt> MQTT { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Motion>().HasNoKey();
        }
    }
}
