using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ProductService.Application.Abstractions.Persistence;
using ProductService.Application.Common.Behaviors;
using ProductService.Application.DependencyInjection;
using ProductService.Application.Features.Commands.CreateProduct;
using ProductService.Infrastructure.DependencyInjection;
using ProductService.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Gọi module
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((doc, req) =>
    {
        // Detect nếu request đến từ Gateway (qua port 5000)
        var isViaGateway = req.Host.Port == 5000 ||
                          req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                          req.Headers["Referer"].ToString().Contains(":5000");

        if (isViaGateway)
        {
            // Force URL qua Gateway
            doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5000/api/product",
                    Description = "Via Gateway"
                }
            };
        }
        else
        {
            // Chạy trực tiếp service
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

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapControllers();


app.Run();