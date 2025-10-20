namespace Identity.Api.Contracts.Auth;

public sealed record RegisterResponse(Guid UserId, string Username, string Email);
