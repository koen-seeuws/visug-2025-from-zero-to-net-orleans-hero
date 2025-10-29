using FluentValidation;

namespace TheCodeKitchen.Application.Validation;

public static class IRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, TProperty> IsInCollection<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        IEnumerable<TProperty> validValues,
        IEqualityComparer<TProperty>? comparer = null,
        string? errorMessage = null)
    {
        return ruleBuilder
            .Must(value => validValues.Contains(value, comparer))
            .WithMessage(errorMessage ?? "{PropertyName} must be one of the allowed values.");
    }
}