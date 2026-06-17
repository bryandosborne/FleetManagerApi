using Microsoft.EntityFrameworkCore;
using FleetManagerApi.Models;

namespace FleetManagerApi.Data
{
    public class FleetManagerApiContext(DbContextOptions<FleetManagerApiContext> options) : DbContext(options)
    {
        public required DbSet<Truck> Trucks { get; set; }
        public required DbSet<Driver> Drivers { get; set; }
        public required DbSet<Load> Loads { get; set; }
    }
}
