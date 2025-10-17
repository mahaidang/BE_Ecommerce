namespace Ordering.Application.Inventory;

public interface IInventoryStockClient
{
    Task<(bool ok, int available, string message)> CheckAsync(Guid productId, int quantity, Guid? warehouseId = null, CancellationToken ct = default);
}
