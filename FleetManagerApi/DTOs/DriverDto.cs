namespace FleetManagerApi.DTOs;

public class DriverDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CurrentLatitude { get; set; }
    public decimal CurrentLongitude { get; set; }
    public double RemainingHours { get; set; } // Flattened to a simple number for JSON clients
}
