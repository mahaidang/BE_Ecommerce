using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace ReportService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection s)
    {
        s.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return s;
    }
}
