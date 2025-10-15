using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpsertItem;

public sealed class UpdateQtyHandler : IRequestHandler<UpdateQtyCommand, Basket>
{
    private readonly IBasketRepository _repo;

    public UpdateQtyHandler(IBasketRepository repo) => _repo = repo;

    public async Task<Basket> Handle(UpdateQtyCommand c, CancellationToken ct)
    {
        var basket = await _repo.GetAsync(c.UserId, ct) ?? new Basket { UserId = c.UserId, Items = new() };
        var it = basket.Items.FirstOrDefault(x => x.ProductId == c.ProductId);
        if (it is null) throw new KeyNotFoundException("Item not found");
        it.Quantity = c.Quantity;
        await _repo.UpsertAsync(basket, c.Ttl, ct);
        return basket;
    }
}