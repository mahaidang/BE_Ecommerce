using Grpc.Core;
using Inventory.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryService.Grpc;

namespace Inventory.Infrastructure.Services;

public sealed class InventoryGrpcService : InventoryService.Grpc.Inventory.InventoryBase
{
    private readonly IInventoryDbContext _db;
    public InventoryGrpcService(IInventoryDbContext db) => _db = db;

    public override async Task<CheckStockResponse> CheckStock(CheckStockRequest req, ServerCallContext context)
    {
        if (req.Quantity <= 0)
            return new CheckStockResponse { Available = false, AvailableQty = 0, Message = "Quantity must be > 0" };

        if (!Guid.TryParse(req.ProductId, out var productId))
            return new CheckStockResponse { Available = false, AvailableQty = 0, Message = "Invalid productId" };

        Guid? warehouseId = null;
        if (!string.IsNullOrWhiteSpace(req.WarehouseId))
        {
            if (!Guid.TryParse(req.WarehouseId, out var w))
                return new CheckStockResponse { Available = false, AvailableQty = 0, Message = "Invalid warehouseId" };
            warehouseId = w;
        }

        // Tính tồn khả dụng = Quantity - ReservedQty
        var query = _db.Stocks.AsNoTracking().Where(s => s.ProductId == productId);
        if (warehouseId.HasValue) query = query.Where(s => s.WarehouseId == warehouseId.Value);

        var availableQty = await query.SumAsync(s => (int?)(s.Quantity - s.ReservedQty)) ?? 0;
        var ok = availableQty >= req.Quantity;

        return new CheckStockResponse
        {
            Available = ok,
            AvailableQty = availableQty,
            Message = ok ? "Stock is sufficient" : "Insufficient stock"
        };
    }
}
