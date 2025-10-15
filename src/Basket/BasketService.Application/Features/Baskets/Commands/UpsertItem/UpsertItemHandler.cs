using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpsertItem;

public sealed class UpsertItemHandler : IRequestHandler<UpsertItemCommand, Basket>
{
    private readonly IBasketRepository _repo;

    public UpsertItemHandler(IBasketRepository repo) => _repo = repo;

    public async Task<Basket> Handle(UpsertItemCommand c, CancellationToken ct)
    {
        var item = new BasketItem
        {
            ProductId = c.ProductId,
            Sku = c.Sku.Trim(),
            Name = c.Name.Trim(),
            UnitPrice = c.UnitPrice,
            Quantity = c.Quantity,
            Currency = c.Currency.Trim().ToUpperInvariant()
        };

        await _repo.AddOrUpdateItemAsync(c.UserId, item, c.Ttl, ct);
        return await _repo.GetAsync(c.UserId, ct) ?? new Basket { UserId = c.UserId, Items = new() };
    }
}
