using System.Text.Json;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using StackExchange.Redis;

namespace BasketService.Infrastructure;

public class RedisBasketRepository : IBasketRepository
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

    // Key format: basket:{userId}
    private static string Key(Guid userId) => $"basket:{userId:D}";

    public RedisBasketRepository(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task<Basket?> GetAsync(Guid userId)
    {
        var s = await _db.StringGetAsync(Key(userId));
        if (s.IsNullOrEmpty) return null;
        return JsonSerializer.Deserialize<Basket>(s!, _json);
    }

    public async Task UpsertAsync(Basket basket, TimeSpan? ttl = null)
    {
        basket.UpdatedAtUtc = DateTime.UtcNow;
        var payload = JsonSerializer.Serialize(basket, _json);
        await _db.StringSetAsync(Key(basket.UserId), payload, ttl);
    }

    public async Task<bool> DeleteAsync(Guid userId) =>
        await _db.KeyDeleteAsync(Key(userId));

    public async Task AddOrUpdateItemAsync(Guid userId, BasketItem item, TimeSpan? ttl = null)
    {
        var basket = await GetAsync(userId) ?? new Basket { UserId = userId };
        var existing = basket.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is null)
            basket.Items.Add(item);
        else
        {
            existing.Quantity = item.Quantity; // cập nhật số lượng
            existing.UnitPrice = item.UnitPrice;
            existing.Sku = item.Sku;
            existing.Name = item.Name;
            existing.Currency = item.Currency;
        }
        await UpsertAsync(basket, ttl);
    }

    public async Task<bool> RemoveItemAsync(Guid userId, Guid productId, TimeSpan? ttl = null)
    {
        var basket = await GetAsync(userId);
        if (basket is null) return false;

        var removed = basket.Items.RemoveAll(i => i.ProductId == productId) > 0;
        if (!removed) return false;

        await UpsertAsync(basket, ttl);
        return true;
    }

    public async Task ClearAsync(Guid userId) =>
        await _db.KeyDeleteAsync(Key(userId));
}
