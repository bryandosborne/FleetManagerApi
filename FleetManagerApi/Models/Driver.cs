namespace FleetManagerApi.Models
{
    public class Driver
    {
        public int Id { get; set; } 
        public int TruckId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DriverLicenseNumber { get; set; } = string.Empty;

    }
}
