using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Application.Interfaces;

public interface IInventoryDbContext
{
    DbSet<Warehouse> Warehouses { get; }
    DbSet<Stock> Stocks { get; }
    DbSet<StockMovement> StockMovements { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
