using BasketService.Domain.Entities;

namespace BasketService.Application.Abstractions.Persistence;

public interface IBasketRepository
{
    Task<Domain.Entities.Basket?> GetAsync(Guid userId, CancellationToken ct);
    Task UpsertAsync(Domain.Entities.Basket basket, TimeSpan? ttl, CancellationToken ct);
    Task<bool> RemoveItemAsync(Guid userId, Guid productId, TimeSpan? ttl, CancellationToken ct);
    Task AddOrUpdateItemAsync(Guid userId, BasketItem item, TimeSpan? ttl, CancellationToken ct);
    Task ClearAsync(Guid userId, CancellationToken ct);
}
