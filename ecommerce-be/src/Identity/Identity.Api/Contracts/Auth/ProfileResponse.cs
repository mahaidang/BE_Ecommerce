namespace Identity.Api.Contracts.Auth;

public sealed record ProfileResponse(Guid UserId, string Username, string Email);
