using System.Text.Json;
using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using StackExchange.Redis;

namespace BasketService.Infrastructure.Persistence;

public sealed class RedisBasketRepository : IBasketRepository
{
    private readonly IDatabase _db;
    private static string Key(Guid userId) => $"basket:{userId:D}";

    private static readonly JsonSerializerOptions JsonOpt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RedisBasketRepository(IConnectionMultiplexer mux)
        => _db = mux.GetDatabase();

    public async Task<Domain.Entities.Basket?> GetAsync(Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var value = await _db.StringGetAsync(Key(userId));
        if (value.IsNullOrEmpty) return null;

        ct.ThrowIfCancellationRequested();
        return JsonSerializer.Deserialize<Domain.Entities.Basket>(value!, JsonOpt);
    }

    public async Task UpsertAsync(Domain.Entities.Basket basket, TimeSpan? ttl, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        basket.UpdatedAtUtc = DateTime.UtcNow;
        var payload = JsonSerializer.Serialize(basket, JsonOpt);

        // StackExchange.Redis chưa nhận CancellationToken ở các API phổ biến
        await _db.StringSetAsync(Key(basket.UserId), payload, ttl);

        ct.ThrowIfCancellationRequested();
    }

    public async Task<bool> RemoveItemAsync(Guid userId, Guid productId, TimeSpan? ttl, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var basket = await GetAsync(userId, ct);
        if (basket is null) return false;

        basket.Items ??= new List<BasketItem>();
        var removed = basket.Items.RemoveAll(i => i.ProductId == productId) > 0;
        if (!removed) return false;

        await UpsertAsync(basket, ttl, ct);
        return true;
    }

    public async Task AddOrUpdateItemAsync(Guid userId, BasketItem item, TimeSpan? ttl, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var basket = await GetAsync(userId, ct) ?? new Domain.Entities.Basket { UserId = userId, Items = new() };

        var existing = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is null)
        {
            basket.Items.Add(item);
        }
        else
        {
            existing.Sku = item.Sku;
            existing.Name = item.Name;
            existing.UnitPrice = item.UnitPrice;
            existing.Quantity = item.Quantity;
            existing.Currency = item.Currency;
        }

        await UpsertAsync(basket, ttl, ct);
    }

    public async Task ClearAsync(Guid userId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        await _db.KeyDeleteAsync(Key(userId));
    }
}
