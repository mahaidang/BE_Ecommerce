using Inventory.Application.Interfaces;
using Inventory.Infrastructure.Models;
using Inventory.Infrastructure.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Inventory.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // ✅ EF Core
        services.AddDbContext<InventoryDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("Default")));

        // Fix for CS0266 and CS1662:
        // Use an explicit cast to IInventoryDbContext when resolving InventoryDbContext.
        services.AddScoped<IInventoryDbContext>(sp => (IInventoryDbContext)sp.GetRequiredService<InventoryDbContext>());

        // ✅ RabbitMQ
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
        {
            HostName = config["RabbitMq:Host"] ?? "rabbitmq",
            Port = int.Parse(config["RabbitMq:Port"] ?? "5672"),
            UserName = config["RabbitMq:User"] ?? "guest",
            Password = config["RabbitMq:Pass"] ?? "guest",
            DispatchConsumersAsync = true
        });

        services.AddSingleton<IConnection>(sp =>
            sp.GetRequiredService<IConnectionFactory>().CreateConnection());

        // ✅ Background Worker (Saga)
        services.AddHostedService<InventorySagaConsumer>();

        // ✅ gRPC
        services.AddGrpc();

        return services;
    }
}
