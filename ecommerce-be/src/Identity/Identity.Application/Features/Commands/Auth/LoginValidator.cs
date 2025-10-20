using FluentValidation;
using Identity.Application.Features.Commands.Auth;

namespace Identity.Application.Features.Users.Commands.Login;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Dto.UsernameOrEmail).NotEmpty();
        RuleFor(x => x.Dto.Password).NotEmpty();
    }
}
