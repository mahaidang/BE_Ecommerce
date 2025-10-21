using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Report.Application.Abstractions.Persistence;
using Report.Application.Features.Queries;
using ReportService.Application;
using ReportService.Infrastructure;
using ReportService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReportDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IReportDbContext>(sp => sp.GetRequiredService<ReportDbContext>());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetOrderStatusCountsQuery).Assembly));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((doc, req) =>
    {
        var isViaGateway = req.Host.Port == 5000 ||
                           req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                           req.Headers["Referer"].ToString().Contains(":5000");

        if (isViaGateway)
        {
            doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = "http://localhost:5000/api/report",
                        Description = "Via Gateway"
                    }
                };
        }
        else
        {
            doc.Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = $"{req.Scheme}://{req.Host.Value}",
                        Description = "Direct Access"
                    }
                };
        }
    });
});

app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK - ReportService"));

app.Run();
