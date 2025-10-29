using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheCodeKitchen.Infrastructure.Extensions;

namespace TheCodeKitchen.Infrastructure.AzureSignalR;

public static class SignalRServiceRegistration
{
    public static void AddAzureSignalRServices(this IServiceCollection services,
        IConfiguration configuration,
        string azureSignalRSection = "AzureSignalR")
    {
        var azureSignalRConnectionString =
            configuration.GetConnectionString("AzureSignalR") ??
            throw new InvalidOperationException("ConnectionStrings__AzureSignalR is not configured.");

        var azureSignalRConfiguration = configuration.BindAndValidateConfiguration<
            AzureSignalRConfiguration,
            AzureSignalRConfigurationValidator
        >(azureSignalRSection);

        services
            .AddSignalR()
            .AddAzureSignalR(options =>
            {
                options.ConnectionString = azureSignalRConnectionString;
                options.ApplicationName = azureSignalRConfiguration.ApplicationName;
            });
    }
}