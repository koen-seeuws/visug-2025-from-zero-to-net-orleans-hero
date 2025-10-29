using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Validation;

public sealed class CreateIngredientFormModelValidator : ValidatorBase<CreateIngredientFormModel>
{
    public CreateIngredientFormModelValidator()
    {
        RuleFor(i => i.Name).Length(FoodNameLength.Minimum, FoodNameLength.Maximum);
    }
}