using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Product.Application.Abstractions.Persistence;
using Product.Infrastructure.Repositories;

namespace Product.Infrastructure.DependencyInjection;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Mongo config
        var mongoCfg = config.GetSection("Mongo");
        var connStr = mongoCfg.GetValue<string>("ConnectionString")!;
        var dbName = mongoCfg.GetValue<string>("Database")!;

        // MongoDB DI
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connStr));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));

        // Repository
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddSingleton<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
