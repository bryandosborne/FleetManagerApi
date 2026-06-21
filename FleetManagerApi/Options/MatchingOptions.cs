namespace FleetManagerApi.Options;

public class MatchingOptions
{
    public decimal DistanceWeight { get; set; } = 0.6m;
    public decimal HosWeight { get; set; } = 0.4m;
    public double AverageTruckSpeedMph { get; set; } = 55.0;
}

