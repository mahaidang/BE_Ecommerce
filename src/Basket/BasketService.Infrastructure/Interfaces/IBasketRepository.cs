using BasketService.Domain.Entities;

namespace BasketService.Application.Interfaces;

public interface IBasketRepository
{
    Task<Basket?> GetAsync(Guid userId);
    Task UpsertAsync(Basket basket, TimeSpan? ttl = null);
    Task<bool> DeleteAsync(Guid userId);

    Task AddOrUpdateItemAsync(Guid userId, BasketItem item, TimeSpan? ttl = null);
    Task<bool> RemoveItemAsync(Guid userId, Guid productId, TimeSpan? ttl = null);
    Task ClearAsync(Guid userId);
}
