using FluentValidation;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Application.Validation;
using TheCodeKitchen.Presentation.ManagementUI.Models.FormModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Validation;

public sealed class CreateGameFormModelValidator : ValidatorBase<CreateGameFormModel>
{
    public CreateGameFormModelValidator()
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(g => g.TimePerMoment)
            .InclusiveBetween(
                TimePerMoment.Minimum,
                TimePerMoment.Maximum
            );
        RuleFor(g => g.SpeedModifier)
            .InclusiveBetween(
                GameSpeedModifier.Minimum,
                GameSpeedModifier.Maximum
            );
        RuleFor(g => g.MinimumTimeBetweenOrders)
            .InclusiveBetween(
                MinimumTimeBetweenOrders.Minimum,
                MinimumTimeBetweenOrders.Maximum
            );
        RuleFor(g => g.MaximumTimeBetweenOrders)
            .InclusiveBetween(
                MaximumTimeBetweenOrders.Minimum,
                MaximumTimeBetweenOrders.Maximum
            );
        RuleFor(g => g.MinimumItemsPerOrder)
            .InclusiveBetween(
                ItemsPerOrder.Minimum,
                ItemsPerOrder.Maximum
            )
            .LessThanOrEqualTo(g => g.MaximumItemsPerOrder);
        RuleFor(g => g.MaximumItemsPerOrder)
            .InclusiveBetween(
                ItemsPerOrder.Minimum,
                ItemsPerOrder.Maximum
            )
            .GreaterThanOrEqualTo(g => g.MinimumItemsPerOrder);
        RuleFor(g => g.OrderSpeedModifier)
            .InclusiveBetween(
                OrderSpeedModifier.Minimum,
                OrderSpeedModifier.Maximum
            );
    }
}