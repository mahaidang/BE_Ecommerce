namespace Identity.Application.Features.Commands.Auth;

public record RegisterDto(string Username, string Email, string Password);
public record LoginDto(string UsernameOrEmail, string Password);
