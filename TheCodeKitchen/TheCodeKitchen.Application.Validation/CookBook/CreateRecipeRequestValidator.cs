using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Contracts.Requests.CookBook;

namespace TheCodeKitchen.Application.Validation.CookBook;

public sealed class CreateRecipeValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeValidator(
        RecipeIngredientDtoValidator recipeIngredientDtoValidator,
        RecipeStepDtoValidator recipeStepDtoValidator
    )
    {
        RuleFor(r => r.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
        RuleFor(r => r.Ingredients)
            .NotEmpty()
            .Must(i => i.Count >= 2)
            .WithMessage("At least two ingredients are required.")
            .Must(i => i.Count == i.DistinctBy(ri => ri.Name, StringComparer.OrdinalIgnoreCase).Count())
            .WithMessage("Duplicate ingredients are not allowed.");
        RuleForEach(r => r.Ingredients).SetValidator(recipeIngredientDtoValidator);
        RuleForEach(r => r.Steps).SetValidator(recipeStepDtoValidator);
    }
}