using Microsoft.EntityFrameworkCore;
using FleetManagerApi.Models;

namespace FleetManagerApi.Data
{
    public class FleetManagerApiContext(DbContextOptions<FleetManagerApiContext> options) : DbContext(options)
    {
        public required DbSet<Truck> Trucks { get; set; }
        public required DbSet<Driver> Drivers { get; set; }
        public required DbSet<Load> Loads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            
            modelBuilder.Entity<Driver>().Property(d => d.FirstName).HasMaxLength(50);
            modelBuilder.Entity<Driver>().Property(d => d.LastName).HasMaxLength(50);
            modelBuilder.Entity<Truck>().Property(t => t.TruckNumber).HasMaxLength(20);
        }
    }
}