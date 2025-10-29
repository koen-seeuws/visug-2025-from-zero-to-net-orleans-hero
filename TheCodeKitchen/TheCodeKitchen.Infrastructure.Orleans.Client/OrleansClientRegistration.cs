using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Infrastructure.Extensions;

namespace TheCodeKitchen.Infrastructure.Orleans.Client;

public static class OrleansClientRegistration
{
    public static void AddTheCodeKitchenOrleansClient(this IServiceCollection services, IConfiguration configuration)
    {
        var clientConfiguration =
            configuration.BindAndValidateConfiguration<OrleansConfiguration, OrleansConfigurationValidator>(
                "TheCodeKitchenOrleans");

        var azureStorageConnectionString =
            configuration.GetConnectionString("AzureStorage") ??
            throw new InvalidOperationException("ConnectionStrings__AzureStorage is not configured.");

        var eventHubConnectionString =
            configuration.GetConnectionString("EventHubNamespace") ??
            throw new InvalidOperationException("ConnectionStrings__EventHubNamespace is not configured.");

        var tableClient = new TableServiceClient(azureStorageConnectionString);

        services.AddOrleansClient(client =>
        {
            client.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = clientConfiguration.ClusterId;
                options.ServiceId = clientConfiguration.ServiceId;
            });

            client.UseAzureStorageClustering(options =>
            {
                options.TableServiceClient = tableClient;
                options.TableName = TheCodeKitchenAzureTableConstants.Clustering;
            });

            client
                .AddStreaming()
                .AddEventHubStreams(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider,
                    eventHubConfigurator =>
                    {
                        eventHubConfigurator.ConfigureEventHub(eventHubBuilder =>
                        {
                            eventHubBuilder.Configure(options =>
                            {
                                options.ConfigureEventHubConnection(
                                    eventHubConnectionString,
                                    clientConfiguration.Streaming?.EventHub,
                                    clientConfiguration.Streaming?.ConsumerGroup
                                );
                            });
                        });
                    });
        });
    }
}