using BasketService.Domain.Entities;
using MediatR;

namespace BasketService.Application.Features.Baskets.Queries;

public sealed record GetBasketQuery(Guid UserId) : IRequest<Domain.Entities.Basket>;