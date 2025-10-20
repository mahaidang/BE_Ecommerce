using MediatR;

namespace Identity.Application.Features.Commands.Auth;

public sealed record RegisterCommand(RegisterDto Dto) : IRequest<RegisterResult>;

public sealed record LoginCommand(LoginDto Dto) : IRequest<LoginResult>;
