using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common;
using Ordering.Domain.Entities;
using System.Text.Json;

namespace Ordering.Application.Orders.Command;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderingDbContext _db;
    public CancelOrderHandler(IOrderingDbContext db) => _db = db;

    // Chỉ cho hủy khi đang Pending/Confirmed
    private static readonly HashSet<string> Cancelable = new(StringComparer.OrdinalIgnoreCase)
        { "Pending", "Confirmed" };

    public async Task<bool> Handle(CancelOrderCommand req, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == req.OrderId, ct);
        if (order is null) return false;
        if (!Cancelable.Contains(order.Status)) return false;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        order.Status = "Cancelled";
        order.UpdatedAtUtc = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(req.Reason))
            order.Note = string.IsNullOrWhiteSpace(order.Note) ? req.Reason : $"{order.Note} | Cancel: {req.Reason}";

        var evt = new { orderId = order.Id, orderNo = order.OrderNo, reason = req.Reason };
        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            EventType = "OrderCancelled",
            Payload = JsonSerializer.Serialize(evt),
            OccurredAtUtc = DateTime.UtcNow
        });

        var changed = await _db.SaveChangesAsync(ct) > 0;
        await tx.CommitAsync(ct);
        return changed;
    }
}
