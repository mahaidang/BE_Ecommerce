using OrchestratorService.Worker.Services;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMq:Host"] ?? "localhost",
    Port = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672"),
    UserName = builder.Configuration["RabbitMq:User"] ?? "guest",
    Password = builder.Configuration["RabbitMq:Pass"] ?? "guest",
    DispatchConsumersAsync = true
});

builder.Services.AddSingleton<IConnection>(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());
builder.Services.AddHostedService<OrchestratorWorker>();

var app = builder.Build();
await app.RunAsync();

