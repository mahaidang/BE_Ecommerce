using BasketService.Application.Abstractions.Persistence;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.Delete;

public sealed class RemoveItemHandler(IBasketRepository repo)
    : IRequestHandler<RemoveItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveItemCommand c, CancellationToken ct)
    {
        var ok = await repo.RemoveItemAsync(c.UserId, c.ProductId, c.Ttl, ct);
        if (!ok) throw new KeyNotFoundException("Item not found");
        return Unit.Value;
    }
}
