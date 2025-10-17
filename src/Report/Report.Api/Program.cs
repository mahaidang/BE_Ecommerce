using Microsoft.EntityFrameworkCore;
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
    cfg.RegisterServicesFromAssembly(typeof(GetDashboardStatsQuery).Assembly));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK - ReportService"));

app.Run();
