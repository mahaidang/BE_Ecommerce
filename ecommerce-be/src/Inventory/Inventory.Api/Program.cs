using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Models;
using Inventory.Infrastructure.Services;
using InventoryService.Application.DependencyInjection;
using Inventory.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load 2 module chính
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();              // ⬅️ thêm gRPC


builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());


var app = builder.Build();

//if (app.Environment.IsDevelopment()) { 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
//}

// ⬅️ Map gRPC endpoint
app.MapGrpcService<InventoryGrpcService>();
app.MapGet("/", () => "Inventory gRPC up. Use a gRPC client to call.");

app.MapGet("/health", () => Results.Ok("OK - Inventory"));

app.Run();
