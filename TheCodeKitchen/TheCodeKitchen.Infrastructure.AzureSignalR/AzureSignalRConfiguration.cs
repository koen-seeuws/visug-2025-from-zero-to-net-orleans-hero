using FluentValidation;

namespace TheCodeKitchen.Infrastructure.AzureSignalR;

public sealed class AzureSignalRConfiguration
{
    public string? ApplicationName { get; set; } = null;
}

public sealed class AzureSignalRConfigurationValidator : AbstractValidator<AzureSignalRConfiguration>
{
    public AzureSignalRConfigurationValidator()
    {

    }
}