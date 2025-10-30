using Basket.Application.Abstractions.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;


namespace Basket.Infrastructure.External;

public static class ProductApiInstaller
{
    public static IServiceCollection AddProductApiClient(this IServiceCollection services, IConfiguration config)
    {
        var baseUrl = config["ProductApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("ProductApi:BaseUrl not configured");

        services.AddRefitClient<IProductApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseUrl);
                c.Timeout = TimeSpan.FromSeconds(10);
            });

        return services;
    }
}