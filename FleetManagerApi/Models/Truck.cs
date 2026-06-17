namespace FleetManagerApi.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string TruckNumber { get; set; } = string.Empty;
        public int DriverId { get; set; }

    }
}
