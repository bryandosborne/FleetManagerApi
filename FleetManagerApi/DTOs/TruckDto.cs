namespace FleetManagerApi.DTOs;

public class TruckDto
{
    public int Id { get; set; }
    public string TruckNumber { get; set; } = string.Empty;
    public int CurrentDriverId { get; set; }
}
