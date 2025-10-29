using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Validation;

public sealed class IngredientFormModelValidator : AbstractValidator<IngredientFormModel>
{
    public IngredientFormModelValidator(StepFormModelValidator stepFormModelValidator)
    {
        RuleFor(i => i.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
        RuleForEach(i => i.Steps).SetValidator(stepFormModelValidator);
    }
}