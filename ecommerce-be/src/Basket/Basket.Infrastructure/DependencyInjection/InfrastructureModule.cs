using Autofac;
using System.Reflection;
using Module = Autofac.Module;

namespace Basket.Infrastructure.DependencyInjection;

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
