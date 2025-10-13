using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpsertItem;

public sealed class UpsertItemHandler(IBasketRepository repo)
    : IRequestHandler<UpsertItemCommand, Basket>
{
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

        await repo.AddOrUpdateItemAsync(c.UserId, item, c.Ttl, ct);
        return await repo.GetAsync(c.UserId, ct) ?? new Basket { UserId = c.UserId, Items = new() };
    }
}
