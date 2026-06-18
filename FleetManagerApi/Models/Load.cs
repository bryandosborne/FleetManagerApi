using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagerApi.Models
{
    public class Load
    {
        public int Id { get; set; }
        public int TruckId { get; set; }
        public string BOL { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public string PickupLocation { get; set; } = string.Empty;
        public string DeliveryLocation { get; set; } = string.Empty;
        public LoadStatus Status { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? PickupDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DeliveryDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? ETA { get; set; }        
        
    }
}
