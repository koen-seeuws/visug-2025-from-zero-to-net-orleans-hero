using FluentValidation;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;

namespace TheCodeKitchen.Application.Validation.Cook;

public sealed class SendMessageValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageValidator()
    {
        RuleFor(m => m.Content).NotEmpty();
    }
}