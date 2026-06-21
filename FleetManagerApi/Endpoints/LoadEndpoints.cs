using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FleetManagerApi.Data;
using FleetManagerApi.Models;

public static class LoadEndpoints
{
    public static void MapLoadEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Load").WithTags(nameof(Load));

        group.MapGet("/", async (FleetManagerApiContext db) =>
        {
            return await db.Loads.ToListAsync();
        })
        .WithName("GetAllLoads");

        group.MapGet("/{id}", async Task<Results<Ok<Load>, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            return await db.Loads.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Load model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetLoadById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Load load, FleetManagerApiContext db) =>
        {
            var affected = await db.Loads
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Id, load.Id)
                .SetProperty(m => m.TruckId, load.TruckId)
                .SetProperty(m => m.BOL, load.BOL)
                .SetProperty(m => m.Description, load.Description)
                .SetProperty(m => m.CurrentLatitude, load.CurrentLatitude)
                .SetProperty(m => m.CurrentLongitude, load.CurrentLongitude)
                .SetProperty(m => m.PickupLatitude, load.PickupLatitude)
                .SetProperty(m => m.PickupLongitude, load.PickupLongitude)
                .SetProperty(m => m.DeliveryLatitude, load.DeliveryLatitude)
                .SetProperty(m => m.DeliveryLongitude, load.DeliveryLongitude)
                .SetProperty(m => m.Status, load.Status)
                .SetProperty(m => m.PickupDate, load.PickupDate)
                .SetProperty(m => m.DeliveryDate, load.DeliveryDate)
                .SetProperty(m => m.ETA, load.ETA)
        );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateLoad");

        group.MapPost("/", async (Load load, FleetManagerApiContext db) =>
        {
            db.Loads.Add(load);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Load/{load.Id}",load);
        })
        .WithName("CreateLoad");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            var affected = await db.Loads
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteLoad");
    }
}
