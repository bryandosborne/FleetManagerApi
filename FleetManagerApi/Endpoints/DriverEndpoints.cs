using FleetManagerApi.DTOs;
using FleetManagerApi.Models;
using FleetManagerApi.Services;

public static class DriverEndpoints
{
    public static void MapDriverEndpoints(
        this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/Driver")
            .WithTags(nameof(Driver));

        // 1. Get All Drivers
        group.MapGet("/", async (
            IDriverService driverService) =>
        {
            var drivers = await driverService
                .GetAllDriversAsync();

            var dtos = drivers.Select(d =>
                new DriverDto
                {
                    Id = d.Id,
                    Name = $"{d.FirstName} {d.LastName}",
                    CurrentLatitude = d.CurrentLatitude,
                    CurrentLongitude = d.CurrentLongitude,
                    RemainingHours = d.RemainingHours.TotalHours
                }).ToList();

            return TypedResults.Ok(dtos);
        })
        .WithName("GetAllDrivers");

        // 2. Get Driver By ID
        group.MapGet("/{id}", async (
            int id,
            IDriverService driverService) =>
        {
            var d = await driverService
                .GetDriverByIdAsync(id);

            if (d is null)
                return Results.NotFound();

            var dto = new DriverDto
            {
                Id = d.Id,
                Name = $"{d.FirstName} {d.LastName}",
                CurrentLatitude = d.CurrentLatitude,
                CurrentLongitude = d.CurrentLongitude,
                RemainingHours = d.RemainingHours.TotalHours
            };

            return TypedResults.Ok(dto);
        })
        .WithName("GetDriverById");

        // 3. Update Driver
        group.MapPut("/{id}", async (
            int id,
            DriverDto dto,
            IDriverService driverService) =>
        {
            var driverEntity = new Driver
            {
                Id = id,
                CurrentLatitude = dto.CurrentLatitude,
                CurrentLongitude = dto.CurrentLongitude,
                RemainingHours = TimeSpan
                    .FromHours(dto.RemainingHours)
            };

            var updated = await driverService
                .UpdateDriverAsync(id, driverEntity);

            if (updated is null)
                return Results.NotFound();

            return TypedResults.Ok(dto);
        })
        .WithName("UpdateDriver");
    }
}
