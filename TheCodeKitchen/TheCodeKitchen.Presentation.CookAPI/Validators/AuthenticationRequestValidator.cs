using FluentValidation;
using TheCodeKitchen.Presentation.API.Cook.Models;

namespace TheCodeKitchen.Presentation.API.Cook.Validators;

public sealed class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        RuleFor(a => a.Username).NotEmpty().MaximumLength(15);
        RuleFor(a => a.Password).NotEmpty();
    }
}