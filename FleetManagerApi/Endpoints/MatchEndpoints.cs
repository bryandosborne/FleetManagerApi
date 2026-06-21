using FleetManagerApi.Data;
using FleetManagerApi.DTOs;
using FleetManagerApi.Models;
using FleetManagerApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FleetManagerApi.Endpoints
{
    public static class MatchEndpoints
    {
        public static void MapMatchEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/match").WithTags("Load Matching Engine");

            // GET: /api/match/load/{loadId}
            group.MapGet("/load/{loadId:int}", async (int loadId, FleetManagerApiContext db, ILoadMatchingEngine matchingEngine) =>
            {
                // 1. Fetch the requested load
                var load = await db.Loads.FindAsync(loadId);
                if (load == null)
                {
                    return Results.NotFound($"Load with ID {loadId} was not found.");
                }

                // 2. Fetch all drivers eagerly including their live tracking log events
                // This ensures our HOS and location properties are populated
                var drivers = await db.Drivers
                    .Include(d => d.LogEvents)
                    .ToListAsync();

                if (!drivers.Any())
                {
                    return Results.BadRequest("No drivers are currently registered in the system to evaluate.");
                }

                // 3. Process each driver through the DriverService tracking properties
                // (This acts as the real-time telemetry processing before scoring)
                var calculator = new Services.HOS.HoursOfServiceCalculator();
                var evaluationTime = DateTime.UtcNow;

                foreach (var driver in drivers)
                {
                    // Calculate live HOS limits
                    var hosLimits = calculator.CalculateRemainingHours(driver.LogEvents, evaluationTime);
                    driver.RemainingHours = hosLimits.RemainingDrivingTime;

                    // Bind spatial vectors from their latest sequential tracking entry
                    var latestLog = driver.LogEvents
                        .OrderByDescending(l => l.Timestamp)
                        .FirstOrDefault();

                    if (latestLog != null)
                    {
                        driver.CurrentLatitude = latestLog.Latitude;
                        driver.CurrentLongitude = latestLog.Longitude;
                        driver.CurrentStatus = latestLog.Status;
                    }
                }

                // 4. Pass our prepared data arrays into our decoupled business logic engine
                DriverMatchResult matchResult = matchingEngine.FindBestDriverForLoad(load, drivers);

                if (matchResult.BestDriver == null)
                {
                    return Results.Ok(new
                    {
                        Message = "Evaluation complete. No drivers have sufficient Hours of Service (HOS) to accept this dispatch.",
                        Result = matchResult
                    });
                }

                return Results.Ok(matchResult);
            })
            .WithName("GetBestDriverForLoad");


            // POST: /api/match/assign
            group.MapPost("/assign", async (AssignLoadRequest request, FleetManagerApiContext db) =>
            {
                // 1. Fetch the target load
                var load = await db.Loads.FindAsync(request.LoadId);
                if (load == null)
                {
                    return Results.NotFound($"Load with ID {request.LoadId} was not found.");
                }

                // 2. Fetch the target driver
                var driver = await db.Drivers.FindAsync(request.DriverId);
                if (driver == null)
                {
                    return Results.BadRequest($"Driver with ID {request.DriverId} does not exist.");
                }

                // 3. Verification: Ensure the load isn't already assigned or completed
                // (Note: Adjust 'LoadStatus.Assigned' based on your actual LoadStatus enum values)
                if (load.Status == LoadStatus.Assigned)
                {
                    return Results.Conflict($"Load {request.LoadId} is already assigned to another driver.");
                }

                // 4. Update the Load Entity with the assignment vectors
                load.DriverId = driver.Id;
                load.Status = LoadStatus.Assigned;
                load.UpdatedAt = DateTime.UtcNow;

                // 5. Commit the transaction to PostgreSQL
                await db.SaveChangesAsync();

                // Return a clean confirmation DTO back to the dispatch dashboard UI
                return Results.Ok(new
                {
                    Message = $"Successfully dispatched load {load.Id} ({load.Description}) to driver {driver.FirstName} {driver.LastName}.",
                    LoadId = load.Id,
                    DriverId = driver.Id,
                    AssignedAt = DateTime.UtcNow
                });
            })
            .WithName("AssignDriverToLoad");
        }


    // ====================================================================
    // INPUT DATA CONTRACTS (Request DTOs)
    // ====================================================================
    public class AssignLoadRequest
        {
            public int LoadId { get; set; }
            public int DriverId { get; set; }
        }
    }
}
