namespace Identity.Api.Contracts.Auth;

public sealed record RegisterRequest(string Username, string Email, string Password);
