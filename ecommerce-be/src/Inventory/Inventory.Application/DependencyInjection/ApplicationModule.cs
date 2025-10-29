using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Application.DependencyInjection;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
