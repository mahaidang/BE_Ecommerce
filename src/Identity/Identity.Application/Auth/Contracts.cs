namespace Identity.Application.Auth;

public sealed record RegisterCommand(string Username, string Email, string Password) : MediatR.IRequest<RegisterResult>;
public sealed record RegisterResult(Guid UserId, string Username, string Email);

public sealed record LoginCommand(string UsernameOrEmail, string Password) : MediatR.IRequest<LoginResult>;
public sealed record LoginResult(Guid UserId, string Username, string Email);
