using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Contracts.Models.Recipe;

namespace TheCodeKitchen.Application.Validation.CookBook;

public sealed class RecipeStepDtoValidator : AbstractValidator<RecipeStepDto>
{
    public RecipeStepDtoValidator()
    {
        RuleFor(s => s.EquipmentType)
            .IsInCollection(EquipmentType.Steppable, StringComparer.OrdinalIgnoreCase);
        RuleFor(s => s.Time).GreaterThan(TimeSpan.Zero);
    }
}