using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.UpsertItem;

public sealed record UpsertItemCommand(
    Guid UserId, Guid ProductId, string Sku, string Name,
    decimal UnitPrice, int Quantity, string Currency, TimeSpan? Ttl
) : IRequest<Basket>;