using FleetManagerApi.Data;
using FleetManagerApi.DTOs;
using FleetManagerApi.Models;
using FleetManagerApi.Services;
using FleetManagerApi.Services.HOS;
using Microsoft.EntityFrameworkCore;

namespace FleetManagerApi.Endpoints
{
    public static class MatchEndpoints
    {
        public static void MapMatchEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/match").WithTags("Load Matching Engine");

            // GET: /api/match/load/{loadId}
            group.MapGet("/load/{loadId:int}", static async (int loadId, FleetManagerApiContext db, ILoadMatchingEngine matchingEngine) =>
            {
                // 1. Fetch the requested load
                var load = await db.Loads.FindAsync(loadId);
                if (load == null)
                {
                    return Results.NotFound($"Load with ID {loadId} was not found.");
                }

                // 2. Fetch all drivers eagerly including their live tracking log events
                var drivers = await db.Drivers
                    .Include(d => d.LogEvents)
                    .ToListAsync();

                if (!drivers.Any())
                {
                    return Results.BadRequest("No drivers are currently registered in the system to evaluate.");
                }

                // 3. Process each driver through the live tracking properties
                var calculator = new HoursOfServiceCalculator();
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

                // 4. Pass our prepared data arrays into our business logic engine
                DriverMatchResult matchResult = matchingEngine.FindBestDriverForLoad(load, drivers);

                if (matchResult.BestDriver == null)
                {
                    return Results.Ok(new DriverMatchResultDto
                    {
                        Message = "Evaluation complete. No drivers have sufficient Hours of Service (HOS) to accept this dispatch.",
                        BestDriverId = null,
                        BestDriverName = string.Empty,
                        Score = 0
                    });
                }

                // 5. Explicitly map our Domain Match Entity to a clean Public DTO
                return Results.Ok(new DriverMatchResultDto
                {
                    Message = "Optimal match found successfully.",
                    BestDriverId = matchResult.BestDriver.Id,
                    BestDriverName = $"{matchResult.BestDriver.FirstName} {matchResult.BestDriver.LastName}",
                    Score = matchResult.Score
                });
            })
            .WithName("GetBestDriverForLoad");


            // POST: /api/match/assign
            group.MapPost("/assign", static async (AssignLoadRequest request, FleetManagerApiContext db) =>
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

                // 6. Return a dedicated, structured response DTO instead of an anonymous type
                return Results.Ok(new LoadAssignmentResponseDto
                {
                    Message = $"Successfully dispatched load {load.Id} ({load.Description}) to driver {driver.FirstName} {driver.LastName}.",
                    LoadId = load.Id,
                    DriverId = driver.Id,
                    AssignedAt = load.UpdatedAt ?? DateTime.UtcNow
                });
            })
            .WithName("AssignDriverToLoad")
            .RequireAuthorization();
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
