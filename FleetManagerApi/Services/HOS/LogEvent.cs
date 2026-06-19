using System;
using System.Text.Json.Serialization;
using FleetManagerApi.Models;

namespace FleetManagerApi.Services.HOS
{
    public class LogEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public DutyStatus Status { get; set; }

        // Stores exact precision GPS coordinates natively in the database
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public int DriverId { get; set; }

        [JsonIgnore]
        public Driver? Driver { get; set; }
    }
}

