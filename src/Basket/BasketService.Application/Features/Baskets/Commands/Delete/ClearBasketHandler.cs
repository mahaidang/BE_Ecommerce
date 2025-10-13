using BasketService.Application.Abstractions.Persistence;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.Delete;

public sealed class ClearBasketHandler(IBasketRepository repo)
    : IRequestHandler<ClearBasketCommand, Unit>
{
    public async Task<Unit> Handle(ClearBasketCommand c, CancellationToken ct)
    {
        await repo.ClearAsync(c.UserId, ct);
        return Unit.Value;
    }
}