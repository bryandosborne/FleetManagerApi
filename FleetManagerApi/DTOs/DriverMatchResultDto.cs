namespace FleetManagerApi.DTOs;

public class DriverMatchResultDto
{
    public string Message { get; set; } = string.Empty;
    public int? BestDriverId { get; set; }
    public string BestDriverName { get; set; } = string.Empty;
    public decimal Score { get; set; }
}

