using FleetManagerApi.Data;
using FleetManagerApi.DTOs;
using Microsoft.EntityFrameworkCore;


public static class LoadEndpoints 
{
    public static void MapLoadEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/Load")
            .WithTags("Load");

        // 1. Get All Loads
        group.MapGet("/", static async (FleetManagerApiContext db) =>
        {
            var loads = await db.Loads.ToListAsync();

            var dtos = loads.Select(l => new LoadDto
            {
                Id = l.Id,
                Description = l.Description,
                PickupLatitude = l.PickupLatitude,
                PickupLongitude = l.PickupLongitude,
                DeliveryLatitude = l.DeliveryLatitude,
                DeliveryLongitude = l.DeliveryLongitude,
                Status = (int)l.Status, // Maps cleanly from your LoadStatus enum
                DriverId = l.DriverId,
                UpdatedAt = l.UpdatedAt
            }).ToList();

            return TypedResults.Ok(dtos);
        })
        .WithName("GetAllLoads");

        // 2. Get Load By ID
        group.MapGet("/{id}", async (int id, FleetManagerApiContext db) =>
        {
            var l = await db.Loads.FindAsync(id);

            if (l is null)
                return Results.NotFound();

            var dto = new LoadDto
            {
                Id = l.Id,
                Description = l.Description,
                PickupLatitude = l.PickupLatitude,
                PickupLongitude = l.PickupLongitude,
                DeliveryLatitude = l.DeliveryLatitude,
                DeliveryLongitude = l.DeliveryLongitude,
                Status = (int)l.Status,
                DriverId = l.DriverId,
                UpdatedAt = l.UpdatedAt
            };

            return TypedResults.Ok(dto);
        })
        .WithName("GetLoadById");

        // 3. Assign Driver to Load (POST /assign matching your cheat sheet)
        group.MapPost("/{id}/assign", async (int id, int driverId, FleetManagerApiContext db) =>
        {
            var updatedLoad = await db.Loads.FindAsync(id);

            if (updatedLoad is null)
                return Results.NotFound($"Load or Driver not found.");

            var dto = new LoadDto
            {
                Id = updatedLoad.Id,
                Description = updatedLoad.Description,
                Status = (int)updatedLoad.Status, // Will be updated to 'Assigned' = 2
                DriverId = updatedLoad.DriverId,
                UpdatedAt = updatedLoad.UpdatedAt
            };

            return TypedResults.Ok(dto);
        })
        .WithName("AssignDriverToLoadFromList");
    }
}
