using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReportService.Application.Abstractions.Persistence;
using ReportService.Application.Abstractions.Reports;
using ReportService.Infrastructure.Persistence;
using ReportService.Infrastructure.Reports;

namespace ReportService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, IConfiguration cfg)
    {
        s.AddDbContext<ReportDbContext>(o =>
            o.UseSqlServer(cfg.GetConnectionString("Default")));

        s.AddScoped<IReportRepository, SqlReportRepository>();
        s.AddScoped<IExcelReportService, ExcelReportService>();

        return s;
    }
}
