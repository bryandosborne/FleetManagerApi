using FleetManagerApi.Data;
using FleetManagerApi.DTOs;
using FleetManagerApi.Models;
using Microsoft.EntityFrameworkCore;

public static class TruckEndpoints
{
    public static void MapTruckEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Truck").WithTags("Truck");

        // 1. Get All Trucks
        group.MapGet("/", static async (FleetManagerApiContext db) =>
        {
            var trucks = await db.Trucks.ToListAsync();

            var dtos = trucks.Select(t => new TruckDto
            {
                Id = t.Id,
                TruckNumber = t.TruckNumber,
                CurrentDriverId = t.DriverId
            }).ToList();

            return Results.Ok(dtos);
        })
        .WithName("GetAllTrucks");

        // 2. Get Truck By ID
        group.MapGet("/{id}", static async (int id, FleetManagerApiContext db) =>
        {
            var t = await db.Trucks.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (t is null)
                return Results.NotFound($"Truck with ID {id} was not found.");

            var dto = new TruckDto
            {
                Id = t.Id,
                TruckNumber = t.TruckNumber,
                CurrentDriverId = t.DriverId
            };

            return Results.Ok(dto);
        })
        .WithName("GetTruckById");

        // 3. Update Truck
        group.MapPut("/{id}", static async (int id, TruckDto dto, FleetManagerApiContext db) =>
        {
            var affected = await db.Trucks
                .Where(m => m.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.TruckNumber, dto.TruckNumber)
                    .SetProperty(m => m.DriverId, dto.CurrentDriverId)
                );

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("UpdateTruck");

        // 4. Create Truck
        group.MapPost("/", static async (TruckDto dto, FleetManagerApiContext db) =>
        {
            var t = new Truck
            {
                TruckNumber = dto.TruckNumber,
                DriverId = dto.CurrentDriverId
            };

            db.Trucks.Add(t);
            await db.SaveChangesAsync();

            dto.Id = t.Id;
            return Results.Created($"/api/Truck/{t.Id}", dto);
        })
        .WithName("CreateTruck");

        // 5. Delete Truck
        group.MapDelete("/{id}", static async (int id, FleetManagerApiContext db) =>
        {
            var affected = await db.Trucks
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();

            return affected == 1 ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteTruck");
    }
}
