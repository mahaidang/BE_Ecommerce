using Azure.Core;
using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Features.Commands.Auth;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public RegisterHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _hasher = hasher;
        _uow = uow;
    }


    public async Task<RegisterResult> Handle(RegisterCommand req, CancellationToken ct)
    {

        var dto = req.Dto;


        var exists = await _users.ExistsByEmailAsync(dto.Email, ct);
        if (exists) throw new InvalidOperationException("Username or Email already exists");

        var (hash, salt) = _hasher.Hash(req.Dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new RegisterResult(user.Id, user.Username, user.Email);
    }
}
