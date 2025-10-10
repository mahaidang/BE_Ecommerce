using Inventory.Infrastructure.Models;
using InventoryService.Api.Services;
using InventoryService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();              // ⬅️ thêm gRPC


builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// ⬅️ Map gRPC endpoint
app.MapGrpcService<InventoryGrpcService>();
app.MapGet("/", () => "Inventory gRPC up. Use a gRPC client to call.");

app.MapGet("/health", () => Results.Ok("OK - Inventory"));

app.Run();
