using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Contracts.Requests.Pantry;

namespace TheCodeKitchen.Application.Validation.Pantry;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientValidator()
    {
        RuleFor(i => i.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
    }
}