using System;
using System.Collections.Generic;
using FleetManagerApi.Services.HOS; 

namespace FleetManagerApi.Models
{
    public class Driver
    {
        // --- Core Identifiers ---
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DriverLicenseNumber { get; set; } = string.Empty;

        // --- Hardware Tracking ---
        public int? AssignedTruckId { get; set; }

        // --- Current AI Matching Vectors (Spatial & Operational) ---
        public DutyStatus CurrentStatus { get; set; } = DutyStatus.OffDuty;
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }

        // --- Aggregated Hours of Service (Calculated States) ---
        public TimeSpan TotalHoursWorked { get; set; } = TimeSpan.Zero;
        public TimeSpan RemainingHours { get; set; } = TimeSpan.Zero;

        // --- Historical Operational Context ---
        public List<LogEvent> LogEvents { get; set; } = new();
    }
}
