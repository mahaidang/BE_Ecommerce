namespace Identity.Application.Features.Users.Queries.GetCurrentUser;

public sealed record CurrentUserDto(Guid UserId, string Username, string Email);
