using BasketService.Application.Abstractions.Persistence;
using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries;

public sealed class GetBasketHandler : IRequestHandler<GetBasketQuery, Basket>
{
    private readonly IBasketRepository _repo;

    public GetBasketHandler(IBasketRepository repo) => _repo = repo;

    public async Task<Basket> Handle(GetBasketQuery q, CancellationToken ct)
        => await _repo.GetAsync(q.UserId, ct) ?? new Basket { UserId = q.UserId, Items = new() };
}