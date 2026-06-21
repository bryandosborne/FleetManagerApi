namespace FleetManagerApi.DTOs;

public class LoadDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal PickupLatitude { get; set; }
    public decimal PickupLongitude { get; set; }
    public decimal DeliveryLatitude { get; set; }
    public decimal DeliveryLongitude { get; set; }
    public int Status { get; set; } // Represented as an integer state
    public int? DriverId { get; set; } // Nullable if unassigned
    public DateTime? UpdatedAt { get; set; }
}
