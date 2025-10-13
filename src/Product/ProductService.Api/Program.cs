using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ProductService.Application.Abstractions.Persistence;
using ProductService.Application.Common.Behaviors;
using ProductService.Application.Features.Commands.CreateProduct;
using ProductService.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mongo config
var mongoCfg = builder.Configuration.GetSection("Mongo");
var connStr = mongoCfg.GetValue<string>("ConnectionString")!;
var dbName = mongoCfg.GetValue<string>("Database")!;

// Mongo DI
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connStr));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));


// Đăng ký repository
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

//Mediator
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductHandler).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateProductValidator).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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