using FleetManagerApi.Models;
using FleetManagerApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

public static class DriverEndpoints
{
    public static void MapDriverEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Driver").WithTags(nameof(Driver));

        // 1. Get All Drivers
        group.MapGet("/", async (IDriverService driverService) =>
        {
            var drivers = await driverService.GetAllDriversAsync();
            return TypedResults.Ok(drivers);
        })
        .WithName("GetAllDrivers");

        // 2. Get Driver By ID
        group.MapGet("/{id}", async (int id, IDriverService driverService) =>
        { 
            var driver = await driverService.GetDriverByIdAsync(id);
            return driver is not null ? TypedResults.Ok(driver) : Results.NotFound();
        })
        .WithName("GetDriverById");

        // 3. Update Driver
        group.MapPut("/{id}", async (int id, Driver driver, IDriverService driverService) =>
        {
            var updatedDriver = await driverService.UpdateDriverAsync(id, driver);
            return updatedDriver is not null ? TypedResults.Ok(updatedDriver) : Results.NotFound();
        })
        .WithName("UpdateDriver");

        
    }
}
