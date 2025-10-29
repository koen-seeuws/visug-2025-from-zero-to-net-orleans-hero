using Microsoft.Azure.SignalR.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheCodeKitchen.Infrastructure.AzureSignalR.Services;
using TheCodeKitchen.Infrastructure.Extensions;

namespace TheCodeKitchen.Infrastructure.AzureSignalR;

public static class SignalRManagementServiceRegistration
{
    public static void AddSignalRManagementServices(this IServiceCollection services,
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

        // Register ServiceManager as singleton
        services.AddSingleton(_ =>
        {
            return new ServiceManagerBuilder()
                .WithOptions(option =>
                {
                    option.ConnectionString = azureSignalRConnectionString;
                    option.ApplicationName = azureSignalRConfiguration.ApplicationName;
                })
                .BuildServiceManager();
        });

        // Register HubContextProvider as singleton
        services.AddSingleton<HubContextProvider>();

        // Register Realtime services
        services.AddSingleton<IRealTimeGameManagementService, RealTimeGameManagementService>();
        services.AddSingleton<IRealTimeGameService, RealTimeGameService>();
        services.AddSingleton<IRealTimeKitchenService, RealTimeKitchenService>();
        services.AddSingleton<IRealTimeKitchenOrderService, RealTimeKitchenOrderService>();
        services.AddSingleton<IRealTimeCookService, RealTimeCookService>();
    }
}