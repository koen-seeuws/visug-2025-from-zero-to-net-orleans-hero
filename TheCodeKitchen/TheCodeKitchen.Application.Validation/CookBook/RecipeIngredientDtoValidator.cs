using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Contracts.Models.Recipe;

namespace TheCodeKitchen.Application.Validation.CookBook;

public sealed class RecipeIngredientDtoValidator : AbstractValidator<RecipeIngredientDto>
{
    public RecipeIngredientDtoValidator(RecipeStepDtoValidator recipeStepDtoValidator)
    {
        RuleFor(r => r.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
        RuleForEach(i => i.Steps).SetValidator(recipeStepDtoValidator);
    }
}