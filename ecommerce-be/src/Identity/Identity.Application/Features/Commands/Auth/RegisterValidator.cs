using FluentValidation;
using Identity.Application.Features.Commands.Auth;

namespace Identity.Application.Features.Users.Commands.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Dto.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Dto.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Dto.Password).NotEmpty().MinimumLength(6);
    }
}
