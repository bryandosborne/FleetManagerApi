namespace FleetManagerApi.Models
{
    public class Load
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public int TruckId { get; set; }
        public string BOL { get; set; } = string.Empty;
        public DateTime PickupDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ETA { get; set; }
        public string CurrentLocation { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
