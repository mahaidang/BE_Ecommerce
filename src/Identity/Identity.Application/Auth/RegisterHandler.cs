using Identity.Application.Common;
using Identity.Application.Security;
using Identity.Domain;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Auth;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IIdentityDbContext _db;
    public RegisterHandler(IIdentityDbContext db) => _db = db;

    public async Task<RegisterResult> Handle(RegisterCommand req, CancellationToken ct)
    {
        var username = req.Username.Trim();
        var email = req.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Username == username || u.Email == email, ct);
        if (exists) throw new InvalidOperationException("Username or Email already exists");

        var (hash, salt) = PasswordHasher.Hash(req.Password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}
