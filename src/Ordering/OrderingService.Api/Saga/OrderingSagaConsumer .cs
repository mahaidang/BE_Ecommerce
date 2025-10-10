using MediatR;
using OrderingService.Application.Orders.Command;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderingService.Api.Saga;

public sealed class OrderingSagaConsumer : BackgroundService
{
    private readonly ILogger<OrderingSagaConsumer> _log;
    private readonly IConnection _conn;
    private readonly IConfiguration _cfg;
    private readonly IMediator _mediator;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private string Ex => _cfg["RabbitMq:Exchange"] ?? "order.events";

    public OrderingSagaConsumer(ILogger<OrderingSagaConsumer> log, IConnection conn, IConfiguration cfg, IMediator mediator)
    { _log = log; _conn = conn; _cfg = cfg; _mediator = mediator; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ch = _conn.CreateModel();
        ch.ExchangeDeclare(Ex, ExchangeType.Topic, durable: true);
        var q = "ordering.cmd_in";
        ch.QueueDeclare(q, durable: true, exclusive: false, autoDelete: false);
        ch.QueueBind(q, Ex, "cmd.order.update-status");
        ch.QueueBind(q, Ex, "order.confirmed"); // nếu muốn nghe event này để set Confirmed
        ch.BasicQos(0, 50, false);

        var consumer = new AsyncEventingBasicConsumer(ch);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var rk = ea.RoutingKey;
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                _log.LogInformation("Ordering received {rk}: {json}", rk, json);

                if (rk == "cmd.order.update-status")
                {
                    var env = JsonSerializer.Deserialize<EventEnvelope<CmdOrderUpdateStatus>>(json, _json);
                    if (env is not null)
                    {
                        var ok = await _mediator.Send(new UpdateStatusCommand(env.OrderId, env.Data.NewStatus), stoppingToken);
                        _log.LogInformation("UpdateStatus({Status}) -> {Ok}", env.Data.NewStatus, ok);
                    }
                }
                else if (rk == "order.confirmed")
                {
                    var env = JsonSerializer.Deserialize<EventEnvelope<object>>(json, _json);
                    if (env is not null)
                    {
                        var ok = await _mediator.Send(new UpdateStatusCommand(env.OrderId, "Confirmed"), stoppingToken);
                        _log.LogInformation("OrderConfirmed -> UpdateStatus(Confirmed) -> {Ok}", ok);
                    }
                }

                ch.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "OrderingSagaConsumer error");
                ch.BasicNack(ea.DeliveryTag, false, requeue: false);
            }
            await Task.CompletedTask;
        };
        ch.BasicConsume(q, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    // Contracts đồng bộ với Orchestrator
    public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime OccurredAtUtc);
    public record CmdOrderUpdateStatus(Guid OrderId, string NewStatus);
}
