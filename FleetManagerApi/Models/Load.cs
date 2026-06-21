using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagerApi.Models
{
    public class Load
    {
        public int Id { get; set; }
        public int? TruckId { get; set; }

        // 1. ADD THIS FOR THE DRIVER FOREIGN KEY ASSIGNMENT Link
        public int? DriverId { get; set; }

        public string BOL { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public LoadStatus Status { get; set; }

        // --- Pickup Location (GPS Coordinates) ---
        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }

        // --- Delivery Location (GPS Coordinates) ---
        public decimal DeliveryLatitude { get; set; }
        public decimal DeliveryLongitude { get; set; }

        // --- Current Location (GPS Coordinates) ---
        public decimal CurrentLatitude { get; set; }
        public decimal CurrentLongitude { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? PickupDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ETA { get; set; }

        // 2. ADD THIS FOR TRACKING DATA REVISIONS
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedAt { get; set; }
    }
}

