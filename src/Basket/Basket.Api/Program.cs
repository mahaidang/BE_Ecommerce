using BasketService.Application.Abstractions.External;
using BasketService.Application.Features.Baskets.Queries;
using BasketService.Infrastructure.External;
using BasketService.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Autofac;
using Autofac.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket API", Version = "v1" });
});


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());


builder.Services.AddHttpClient("ProductApi", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Services:ProductBaseUrl"]!;
    c.BaseAddress = new Uri(baseUrl);
    c.Timeout = TimeSpan.FromSeconds(5);
});


// Redis
var redisConn = builder.Configuration["Redis:ConnectionString"]!;
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

// Repo
builder.Services.AddScoped<BasketService.Application.Abstractions.Persistence.IBasketRepository, RedisBasketRepository>();

// Đăng ký MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetBasketHandler>());

builder.Services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductServiceUrl"]);
});


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
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
                    Url = "http://localhost:5000/api/basket",
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
//}

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
