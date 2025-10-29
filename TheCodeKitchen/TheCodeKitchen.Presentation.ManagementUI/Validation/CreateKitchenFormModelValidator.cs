using FluentValidation;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Validation;

public sealed class CreateKitchenFormModelValidator : ValidatorBase<CreateKitchenFormModel>
{
    public CreateKitchenFormModelValidator()
    {
        RuleFor(k => k.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}