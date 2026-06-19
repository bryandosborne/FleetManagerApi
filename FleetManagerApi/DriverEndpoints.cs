using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FleetManagerApi.Data;
using FleetManagerApi.Models;
using FleetManagerApi.Services.HOS;

public static class DriverEndpoints
{
    public static void MapDriverEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Driver").WithTags(nameof(Driver));

        group.MapGet("/", async (FleetManagerApiContext db) =>
        {
            return await db.Drivers.ToListAsync();
        })
        .WithName("GetAllDrivers");

        group.MapGet("/{id}", async Task<Results<Ok<Driver>, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            return await db.Drivers.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Driver model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetDriverById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Driver driver, FleetManagerApiContext db) =>
        {
            var affected = await db.Drivers
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Id, driver.Id)
                .SetProperty(m => m.FirstName, driver.FirstName)
                .SetProperty(m => m.LastName, driver.LastName)
                .SetProperty(m => m.DriverLicenseNumber, driver.DriverLicenseNumber)
        );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateDriver");

        group.MapPost("/", async (Driver driver, FleetManagerApiContext db) =>
        {
            db.Drivers.Add(driver);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Driver/{driver.Id}",driver);
        })
        .WithName("CreateDriver");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            var affected = await db.Drivers
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteDriver");

        group.MapGet("/{id}/hos", async Task<Results<Ok<HosRemainingTime>, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            // 1. Fetch the driver and eagerly load their complete history of log events
            var driver = await db.Drivers
                .Include(d => d.LogEvents)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (driver == null)
            {
                return TypedResults.NotFound();
            }

            // 2. Instantiate your calculation service engine
            var calculator = new HoursOfServiceCalculator();

            // 3. Compute the current remaining hours using the driver's log list
            HosRemainingTime remainingTime = calculator.CalculateRemainingHours(driver.LogEvents, DateTime.UtcNow);

            // 4. Return the computed times as a clean JSON payload response
            return TypedResults.Ok(remainingTime);
        })
        .WithName("GetDriverHosCompliance");
    }
}
