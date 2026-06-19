using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FleetManagerApi.Data;
using FleetManagerApi.Models;

public static class TruckEndpoints
{
    public static void MapTruckEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Truck").WithTags(nameof(Truck));

        group.MapGet("/", async (FleetManagerApiContext db) =>
        {
            return await db.Trucks.ToListAsync();
        })
        .WithName("GetAllTrucks");

        group.MapGet("/{id}", async Task<Results<Ok<Truck>, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            return await db.Trucks.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Truck model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetTruckById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Truck truck, FleetManagerApiContext db) =>
        {
            var affected = await db.Trucks
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Id, truck.Id)
                .SetProperty(m => m.TruckNumber, truck.TruckNumber)
                .SetProperty(m => m.DriverId, truck.DriverId)
        );

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateTruck");

        group.MapPost("/", async (Truck truck, FleetManagerApiContext db) =>
        {
            db.Trucks.Add(truck);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Truck/{truck.Id}",truck);
        })
        .WithName("CreateTruck");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, FleetManagerApiContext db) =>
        {
            var affected = await db.Trucks
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteTruck");
    }
}
