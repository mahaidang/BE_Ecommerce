using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
