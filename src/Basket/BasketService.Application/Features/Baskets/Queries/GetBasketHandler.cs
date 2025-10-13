using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries;

public sealed class GetBasketHandler(IBasketRepository repo)
    : IRequestHandler<GetBasketQuery, Basket>
{
    public async Task<Basket> Handle(GetBasketQuery q, CancellationToken ct)
        => await repo.GetAsync(q.UserId, ct) ?? new Basket { UserId = q.UserId, Items = new() };
}