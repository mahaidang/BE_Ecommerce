using MediatR;

namespace Identity.Application.Features.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserDto>;
