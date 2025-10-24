﻿using Identity.Api.Contracts.Auth;
using Identity.Application.Abstractions.Security;
using Identity.Application.Features.Commands.Auth;
using Identity.Application.Features.Users.Queries.GetCurrentUser;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public AuthController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        // Map Request -> Dto -> Command
        var dto = request.Adapt<RegisterDto>();
        var cmd = new RegisterCommand(dto);
        var result = await _sender.Send(cmd, ct);

        // Map Result -> Response
        var response = result.Adapt<RegisterResponse>();
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var dto = request.Adapt<LoginDto>();
        var cmd = new LoginCommand(dto);
        var result = await _sender.Send(cmd, ct);
        var response = result.Adapt<LoginResponse>();
        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Unauthorized();

        var result = await _sender.Send(new GetCurrentUserQuery(_currentUser.UserId.Value), ct);
        return Ok(result.Adapt<ProfileResponse>());
    }
}
