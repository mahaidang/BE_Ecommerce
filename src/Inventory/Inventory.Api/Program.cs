using Inventory.Infrastructure.Models;
using InventoryService.Api.Services;
using InventoryService.Application.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using InventoryService.Api.Saga;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();              // ⬅️ thêm gRPC


builder.Services.AddDbContext<InventoryDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IInventoryDbContext>(sp => sp.GetRequiredService<InventoryDbContext>());

// RabbitMQ
builder.Services.AddSingleton<RabbitMQ.Client.IConnectionFactory>(_ => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMq:Host"] ?? "localhost",
    Port = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672"),
    UserName = builder.Configuration["RabbitMq:User"] ?? "guest",
    Password = builder.Configuration["RabbitMq:Pass"] ?? "guest",
    DispatchConsumersAsync = true
});
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<RabbitMQ.Client.IConnectionFactory>().CreateConnection());

// Worker nhận cmd.inventory.*
builder.Services.AddHostedService<InventorySagaConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// ⬅️ Map gRPC endpoint
app.MapGrpcService<InventoryGrpcService>();
app.MapGet("/", () => "Inventory gRPC up. Use a gRPC client to call.");

app.MapGet("/health", () => Results.Ok("OK - Inventory"));

app.Run();
