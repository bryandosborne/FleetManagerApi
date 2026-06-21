using FleetManagerApi.Data;
using FleetManagerApi.Endpoints;
using FleetManagerApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FleetManagerApiContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDriverService, DriverService>();

builder.Services.AddScoped<ILoadMatchingEngine, LoadMatchingEngine>();


builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapDriverEndpoints();

app.MapTruckEndpoints();

app.MapMatchEndpoints();

app.MapLoadEndpoints();

app.Run();
