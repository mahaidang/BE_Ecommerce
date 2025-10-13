using Identity.Application.Abstractions.Persistence;
using MediatR;

namespace Identity.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserHandler(IUserRepository users)
    : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(request.UserId, ct)
            ?? throw new UnauthorizedAccessException("User not found.");

        return new CurrentUserDto(user.Id, user.Username, user.Email);
    }
}
