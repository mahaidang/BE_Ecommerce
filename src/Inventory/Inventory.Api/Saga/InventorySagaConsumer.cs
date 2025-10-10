using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InventoryService.Api.Saga;

public sealed class InventorySagaConsumer : BackgroundService
{
    private readonly ILogger<InventorySagaConsumer> _log;
    private readonly IConnection _conn;
    private readonly IConfiguration _cfg;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private string Ex => _cfg["RabbitMq:Exchange"] ?? "order.events";

    public InventorySagaConsumer(ILogger<InventorySagaConsumer> log, IConnection conn, IConfiguration cfg)
    { _log = log; _conn = conn; _cfg = cfg; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ch = _conn.CreateModel();
        ch.ExchangeDeclare(Ex, ExchangeType.Topic, durable: true);
        var q = "inventory.cmd_in";
        ch.QueueDeclare(q, durable: true, exclusive: false, autoDelete: false);
        ch.QueueBind(q, Ex, "cmd.inventory.*");  // reserve/release
        ch.BasicQos(0, 50, false);

        var consumer = new AsyncEventingBasicConsumer(ch);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var rk = ea.RoutingKey;
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                _log.LogInformation("Inventory received {rk}: {body}", rk, body);

                if (rk == "cmd.inventory.reserve")
                {
                    var env = JsonSerializer.Deserialize<EventEnvelope<CmdInventoryReserve>>(body, _json);
                    if (env is not null)
                    {
                        // TODO: gọi logic kho thực tế của bạn:
                        //  - kiểm tra tồn đủ cho từng item
                        //  - trừ kho (đặt giữ) hoặc fail
                        var ok = true; // giả lập
                        if (ok)
                        {
                            var evt = new EventEnvelope<InventoryReservedData>(
                                "inventory.stock.reserved",
                                env.CorrelationId, env.OrderId,
                                new InventoryReservedData(Guid.Empty, env.Data.Items.Select(i => new ReservedItem(i.ProductId, i.Quantity)).ToList()),
                                DateTime.UtcNow);
                            Publish(ch, evt.EventType, evt);
                        }
                        else
                        {
                            var evt = new EventEnvelope<InventoryFailedData>(
                                "inventory.stock.failed",
                                env.CorrelationId, env.OrderId,
                                new InventoryFailedData("Out of stock"),
                                DateTime.UtcNow);
                            Publish(ch, evt.EventType, evt);
                        }
                    }
                }
                else if (rk == "cmd.inventory.release")
                {
                    // TODO: cộng trả kho cho các item (nếu bạn có giữ)
                    // Trả về event cũng được, không bắt buộc
                }

                ch.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "InventorySagaConsumer error");
                ch.BasicNack(ea.DeliveryTag, false, requeue: false); // -> DLQ nếu cấu hình
            }
            await Task.CompletedTask;
        };

        ch.BasicConsume(q, autoAck: false, consumer);
        return Task.CompletedTask;
    }

    private void Publish<T>(IModel ch, string rk, EventEnvelope<T> env)
    {
        var props = ch.CreateBasicProperties(); props.ContentType = "application/json"; props.DeliveryMode = 2;
        var json = JsonSerializer.Serialize(env, _json);
        ch.BasicPublish(Ex, rk, props, Encoding.UTF8.GetBytes(json));
    }

    // Contracts (coppy y hệt từ Orchestrator để đồng bộ)
    public record EventEnvelope<T>(string EventType, Guid CorrelationId, Guid OrderId, T Data, DateTime OccurredAtUtc);
    public record ReservedItem(Guid ProductId, int Quantity);
    public record InventoryReservedData(Guid WarehouseId, IReadOnlyList<ReservedItem> Items);
    public record InventoryFailedData(string Reason);
    public record CmdInventoryReserve(Guid OrderId, IReadOnlyList<ReservedItem> Items);
}
