using Identity.Application.Common;
using Identity.Application.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Auth;

public sealed class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityDbContext _db;
    public LoginHandler(IIdentityDbContext db) => _db = db;

    public async Task<LoginResult> Handle(LoginCommand req, CancellationToken ct)
    {
        var input = req.UsernameOrEmail.Trim().ToLowerInvariant();

        var user = await _db.Users
            .Where(u => u.Username == input || u.Email == input)
            .Select(u => new { u.Id, u.Username, u.Email, u.PasswordHash, u.PasswordSalt, u.IsActive })
            .FirstOrDefaultAsync(ct);

        if (user is null || !user.IsActive) throw new UnauthorizedAccessException("Invalid credentials");

        if (!PasswordHasher.Verify(req.Password, user.PasswordHash!, user.PasswordSalt!))
            throw new UnauthorizedAccessException("Invalid credentials");

        return new LoginResult(user.Id, user.Username, user.Email);
    }
}
