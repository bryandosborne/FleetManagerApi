using System;

namespace FleetManagerApi.DTOs;

public class LoadAssignmentResponseDto
{
    public string Message { get; set; } = string.Empty;
    public int LoadId { get; set; }
    public int DriverId { get; set; }
    public DateTime AssignedAt { get; set; }
}

