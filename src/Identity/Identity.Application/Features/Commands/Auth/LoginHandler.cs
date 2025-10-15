using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.Commands.Auth;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _ph;
    private readonly IJwtTokenGenerator _jwt;

    public LoginHandler(IUserRepository users, IPasswordHasher ph, IJwtTokenGenerator jwt)
    {
        _users = users;
        _ph = ph;
        _jwt = jwt;
    }

    public async Task<LoginResult> Handle(LoginCommand req, CancellationToken ct)
    {

        var user = await _users.GetByUsernameOrEmailAsync(req.Dto.UsernameOrEmail, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid username/email");

        // Đúng
        if (!_ph.Verify(req.Dto.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid password");


        var token = _jwt.GenerateToken(user);

        return new LoginResult(user.Id, user.Username, user.Email, token);
    }
}
