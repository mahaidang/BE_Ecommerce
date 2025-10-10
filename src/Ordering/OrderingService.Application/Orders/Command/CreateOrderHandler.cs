using MediatR;
using OrderingService.Application.Common;
using OrderingService.Domain.Entities;
using System.Text.Json;


namespace OrderingService.Application.Orders.Command;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderingDbContext _db;
    public CreateOrderHandler(IOrderingDbContext db) => _db = db;

    public async Task<CreateOrderResult> Handle(CreateOrderCommand req, CancellationToken ct)
    {
        if (req.Items is null || req.Items.Count == 0) 
            throw new ArgumentException("Items empty");

        var subtotal = req.Items.Sum(i => i.UnitPrice * i.Quantity); 
        var grand = subtotal - req.DiscountTotal + req.ShippingFee;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            OrderNo = GenOrderNo(),
            Status = "Pending",                            // Status là string theo EFPT
            Currency = (req.Currency ?? "VND").ToUpperInvariant(),
            Subtotal = subtotal,
            DiscountTotal = req.DiscountTotal,
            ShippingFee = req.ShippingFee,
            GrandTotal = grand,                            // model EFPT không phải computed -> gán giá trị
            Note = req.Note,
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var i in req.Items)
        {
            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = i.ProductId,
                Sku = i.Sku.Trim(),
                ProductName = i.ProductName.Trim(),
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                LineTotal = i.UnitPrice * i.Quantity
            });
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct); // hoặc wrappers
        _db.Orders.Add(order);

        var evt = new
        {
            orderId = order.Id,
            orderNo = order.OrderNo,
            userId = order.UserId,
            status = order.Status,
            currency = order.Currency,
            grandTotal = order.GrandTotal,
            items = order.OrderItems.Select(x => new { x.ProductId, x.Sku, x.ProductName, x.UnitPrice, x.Quantity })
        };

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "OrderCreated",
            Payload = JsonSerializer.Serialize(evt),
            OccurredAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new CreateOrderResult(order.Id, order.OrderNo, order.GrandTotal);
    }

    private static string GenOrderNo()
    {
        var rand = Random.Shared.Next(1000, 9999);
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{rand}";
    }
}
