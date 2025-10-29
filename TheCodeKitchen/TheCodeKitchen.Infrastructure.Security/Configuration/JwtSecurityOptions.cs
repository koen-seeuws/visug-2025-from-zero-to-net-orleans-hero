using FluentValidation;

namespace TheCodeKitchen.Infrastructure.Security.Configuration;

public sealed class JwtSecurityOptions
{
    public string Secret { get; set; } = string.Empty;
    public int ValidForHours { get; set; }
}

public sealed class JwtSecurityOptionsValidator : AbstractValidator<JwtSecurityOptions>
{
    public JwtSecurityOptionsValidator()
    {
        RuleFor(x => x.Secret).NotEmpty().MinimumLength(32);
    }
}