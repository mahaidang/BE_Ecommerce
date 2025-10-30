using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Basket.Infrastructure.External; // Thêm using này để gọi AddProductApiClient
using Module = Autofac.Module;

namespace Basket.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Đăng ký các service khác như Redis, Repository, v.v. tại đây nếu cần

        services.AddProductApiClient(config);  // ✅ Refit client
        return services;
    }
}

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Scan toàn bộ repository, client, persistence
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => t.Name.EndsWith("Repository") || t.Name.EndsWith("Client"))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
    }
}
