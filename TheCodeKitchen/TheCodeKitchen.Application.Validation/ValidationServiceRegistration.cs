using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TheCodeKitchen.Application.Validation;

public static class ValidationServiceRegistration
{
    public static void AddValidationServices(this IServiceCollection services)
    {
        //Fluent Validation
        services.AddValidatorsFromAssembly(typeof(ValidationServiceRegistration).Assembly);
    }
}