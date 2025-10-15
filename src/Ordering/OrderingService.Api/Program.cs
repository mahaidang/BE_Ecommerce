using FluentValidation;
using Grpc.Net.Client;
using InventoryService.Grpc;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderingService.Api.Inventory;
using OrderingService.Api.Outbox;      // ⬅️ để thấy OutboxPublisher
using OrderingService.Api.Saga;
using OrderingService.Application.Common;
using OrderingService.Application.Common.Behaviors;
using OrderingService.Application.Inventory;
using OrderingService.Application.Orders.Command;
using OrderingService.Infrastructure;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;                 // ⬅️ bắt buộc

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* ===== DbContext: dùng Factory, KHÔNG dùng AddDbContext ===== */
builder.Services.AddDbContextFactory<OrderingDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

/* Cấp DbContext theo scope từ Factory (cho Controllers/Handlers) */
builder.Services.AddScoped<OrderingDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<OrderingDbContext>>().CreateDbContext());

/* Nếu bạn có interface IOrderingDbContext thì map về instance ở trên */
builder.Services.AddScoped<IOrderingDbContext>(sp => sp.GetRequiredService<OrderingDbContext>());

//builder.Services.AddDbContext<OrderingDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
//builder.Services.AddScoped<IOrderingDbContext>(sp => sp.GetRequiredService<OrderingDbContext>());

//builder.Services.AddDbContextFactory<OrderingDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSingleton<IInventoryStockClient, GrpcInventoryStockClient>();

// Đăng ký validator
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();

// Đăng ký ValidationBehavior vào MediatR pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderHandler).Assembly));

// RabbitMQ connection (singleton)
builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMq:Host"],
    Port = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672"),
    UserName = builder.Configuration["RabbitMq:User"],
    Password = builder.Configuration["RabbitMq:Pass"],
    DispatchConsumersAsync = true
});
builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());

// Outbox publisher
builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var addr = cfg["Grpc:InventoryBaseUrl"]!;
    var ch = GrpcChannel.ForAddress(addr);
    return new Inventory.InventoryClient(ch);
});

builder.Services.AddHostedService<OrderingSagaConsumer>();

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
                    Url = "http://localhost:5000/api/ordering",
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
}); app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK - Ordering"));

app.MapPost("/debug/rabbit/publish-order-created", async (
    [FromServices] IConnection conn,
    [FromServices] IDbContextFactory<OrderingDbContext> dbFactory,
    [FromServices] IConfiguration cfg,
    [FromQuery] Guid orderId,
    [FromQuery] string? correlationId) =>
{
    if (orderId == Guid.Empty) return Results.BadRequest("orderId is required.");

    await using var db = await dbFactory.CreateDbContextAsync();
    var order = await db.Orders
        .Include(o => o.OrderItems)
        .AsNoTracking()
        .FirstOrDefaultAsync(o => o.Id == orderId);

    if (order is null) return Results.NotFound("Order not found.");

    var env = new
    {
        eventType = "order.created",
        correlationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid() : Guid.Parse(correlationId),
        orderId = order.Id,
        occurredAtUtc = DateTime.UtcNow,
        data = new
        {
            userId = order.UserId,
            currency = order.Currency,
            grandTotal = order.GrandTotal ?? order.Subtotal - order.DiscountTotal + order.ShippingFee,
            items = order.OrderItems.Select(i => new {
                productId = i.ProductId,
                sku = i.Sku,
                name = i.ProductName,
                quantity = i.Quantity,
                unitPrice = i.UnitPrice
            }).ToList()
        }
    };

    var exchange = cfg["RabbitMq:Exchange"] ?? "order.events";
    using var ch = conn.CreateModel();
    ch.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);

    var json = JsonSerializer.Serialize(env, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    var body = Encoding.UTF8.GetBytes(json);
    var props = ch.CreateBasicProperties();
    props.ContentType = "application/json";
    props.DeliveryMode = 2;

    ch.BasicPublish(exchange, routingKey: "order.created", basicProperties: props, body: body);
    return Results.Ok(new { ok = true, exchange, routingKey = "order.created", json });
})
.WithTags("Debug");


// POST /debug/rabbit/declare  -> tạo queue + bind (đỡ phải vào UI)
app.MapPost("/debug/rabbit/declare", (IConnection conn, IConfiguration cfg) =>
{
    var exchange = cfg["RabbitMq:Exchange"] ?? "order.events";
    var queue = "order_created_q";
    var rk = "order.created";

    using var ch = conn.CreateModel();
    ch.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true, autoDelete: false);
    ch.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
    ch.QueueBind(queue, exchange, rk);

    return Results.Ok(new { exchange, queue, routingKey = rk });
})
.WithTags("Debug")
.Produces(StatusCodes.Status200OK);


app.Run();
