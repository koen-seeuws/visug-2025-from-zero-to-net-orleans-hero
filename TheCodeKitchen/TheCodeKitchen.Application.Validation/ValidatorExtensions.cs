using FluentValidation;
using TheCodeKitchen.Application.Contracts.Errors;

namespace TheCodeKitchen.Application.Validation;

public static class ValidatorExtensions
{
    public static bool ValidateAndError<TRequest>(
        this IValidator<TRequest> validator,
        TRequest request,
        out ValidationError validationError)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e =>  e.ErrorMessage)
                .ToArray();
            validationError = new ValidationError(errors); // Your custom error type
            return false;
        }

        validationError = null;
        return true;
    }
}