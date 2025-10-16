using MediatR;

namespace BasketService.Application.Features.Baskets.Commands.Delete;

public sealed record ClearBasketCommand(Guid UserId) : IRequest<Unit>;
