using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Validation;

public sealed class CreateRecipeFormModelValidator : ValidatorBase<CreateRecipeFormModel>
{
    public CreateRecipeFormModelValidator(
        IngredientFormModelValidator ingredientFormModelValidator,
        StepFormModelValidator stepFormModelValidator
    )
    {
        RuleFor(r => r.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
        RuleFor(r => r.Ingredients)
            .NotEmpty()
            .Must(i => i.Count >= 2)
            .WithMessage("At least two ingredients are required.")
            .Must(i => i.Count == i.DistinctBy(ri => ri.Name, StringComparer.OrdinalIgnoreCase).Count())
            .WithMessage("Duplicate ingredients are not allowed.");
        RuleForEach(r => r.Ingredients).SetValidator(ingredientFormModelValidator);
        RuleForEach(r => r.Steps).SetValidator(stepFormModelValidator);
    }
}