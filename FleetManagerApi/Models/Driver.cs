using FleetManagerApi.Services.HOS;

namespace FleetManagerApi.Models
{
    public class Driver
    {
        public int Id { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DriverLicenseNumber { get; set; } = string.Empty;
        public List<LogEvent> LogEvents { get; set; } = new();
        public TimeSpan TotalHoursWorked { get; set; } = new TimeSpan(0); // Tracks total HOS time
        public TimeSpan RemainingHours { get; set; } = new TimeSpan(0); // Calculates remaining HOS time

    }
}
