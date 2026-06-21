using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;
using Microsoft.EntityFrameworkCore;

namespace FleetManagerApi.Data
{
    public class FleetManagerApiContext : DbContext
    {
        public FleetManagerApiContext(DbContextOptions<FleetManagerApiContext> options) : base(options)
        {
        }

        public required DbSet<Truck> Trucks { get; set; }
        public required DbSet<Driver> Drivers { get; set; }
        public required DbSet<Load> Loads { get; set; }
        public required DbSet<LogEvent> LogEvents { get; set; } // 1. ADD THIS LINE

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            
            modelBuilder.Entity<LogEvent>().ToTable("LogEvents");
            modelBuilder.Entity<Driver>().Property(d => d.FirstName).HasMaxLength(50);
            modelBuilder.Entity<Driver>().Property(d => d.LastName).HasMaxLength(50);
            modelBuilder.Entity<Truck>().Property(t => t.TruckNumber).HasMaxLength(20);
        }
    }
}
