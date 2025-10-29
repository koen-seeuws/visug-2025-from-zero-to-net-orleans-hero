using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Orleans.Configuration;
using TheCodeKitchen.Application.Business;
using TheCodeKitchen.Application.Business.Interceptors;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Infrastructure.AzureSignalR;
using TheCodeKitchen.Infrastructure.Extensions;
using TheCodeKitchen.Infrastructure.Logging.Serilog;
using TheCodeKitchen.Infrastructure.Orleans;

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.RegisterSerilog();

// Application Services
builder.Services.AddApplicationServices();

// Infrastructure services
var siloConfiguration =
    builder.Configuration
        .BindAndValidateConfiguration<OrleansConfiguration, OrleansConfigurationValidator>(
            "TheCodeKitchenOrleans");

var azureStorageConnectionString =
    builder.Configuration.GetConnectionString("AzureStorage") ??
    throw new InvalidOperationException("ConnectionStrings__AzureStorage is not configured.");

var eventHubConnectionString =
    builder.Configuration.GetConnectionString("EventHubNamespace") ??
    throw new InvalidOperationException("ConnectionStrings__EventHubNamespace is not configured.");

var tableClient = new TableServiceClient(azureStorageConnectionString);
var blobClient = new BlobServiceClient(azureStorageConnectionString);

#if DEBUG
// TODO: REMOVE, this is only for development purposes to ensure a clean state.
/*
foreach (var storage in TheCodeKitchenAzureTableConstants.All)
{
    tableClient.DeleteTable(storage);
}
*/

//await blobClient.DeleteBlobContainerAsync(siloConfiguration.StateBlobContainer);
//await blobClient.CreateBlobContainerAsync(siloConfiguration.StateBlobContainer);
#endif

builder.Services.AddSignalRManagementServices(builder.Configuration);

builder.UseOrleans(silo =>
{
    silo.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = siloConfiguration.ClusterId;
        options.ServiceId = siloConfiguration.ServiceId;
    });

    silo.UseAzureStorageClustering(options =>
    {
        options.TableServiceClient = tableClient;
        options.TableName = TheCodeKitchenAzureTableConstants.Clustering;
    });
    
    silo.AddIncomingGrainCallFilter<LoggingInterceptor>();

    foreach (var storage in TheCodeKitchenState.All)
    {
        silo.AddAzureBlobGrainStorage(storage, options =>
        {
            options.BlobServiceClient = blobClient;
            options.ContainerName = siloConfiguration.StateBlobContainer;
        });
    }
    
    silo.UseAzureTableReminderService(options =>
    {
        options.TableServiceClient = tableClient;
        options.TableName = TheCodeKitchenAzureTableConstants.Reminders;
    });

    silo.AddAzureTableGrainStorage(TheCodeKitchenAzureTableConstants.PubSubStore, options =>
    {
        options.TableServiceClient = tableClient;
        options.TableName = TheCodeKitchenAzureTableConstants.PubSubStore;
    });

    silo
        .AddStreaming()
        .AddEventHubStreams(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider, eventHubConfigurator =>
        {
            eventHubConfigurator.ConfigureEventHub(eventHubBuilder =>
            {
                eventHubBuilder.Configure(options =>
                {
                    options.ConfigureEventHubConnection(
                        eventHubConnectionString,
                        siloConfiguration.Streaming?.EventHub,
                        siloConfiguration.Streaming?.ConsumerGroup
                    );
                });
            });

            eventHubConfigurator.UseAzureTableCheckpointer(azureTableBuilder =>
                azureTableBuilder.Configure(options =>
                {
                    options.TableServiceClient = tableClient;
                    options.TableName = TheCodeKitchenAzureTableConstants.EventHubCheckpoints;
                })
            );
        });

    silo.UseDashboard();
});

var host = builder.Build();
host.Run();