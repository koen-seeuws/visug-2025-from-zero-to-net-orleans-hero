using FluentValidation;
using TheCodeKitchen.Application.Contracts.Requests.Cook;

namespace TheCodeKitchen.Application.Validation.Cook;

public sealed class StopTimerValidator : AbstractValidator<StopTimerRequest>
{
    public StopTimerValidator()
    {
        RuleFor(s => s.Number).NotEmpty().GreaterThan(0);
    }
}