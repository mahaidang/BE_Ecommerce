namespace Identity.Api;

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string UsernameOrEmail, string Password);
public record AuthResponse(Guid UserId, string Username, string Email, string AccessToken);
