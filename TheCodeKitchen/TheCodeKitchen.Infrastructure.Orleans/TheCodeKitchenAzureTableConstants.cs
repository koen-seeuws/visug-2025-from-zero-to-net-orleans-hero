namespace TheCodeKitchen.Infrastructure.Orleans;

public static class TheCodeKitchenAzureTableConstants
{
    // Clustering
    public const string Clustering = "TheCodeKitchenClustering";

    // Reminders
    public const string Reminders = "TheCodeKitchenReminders";

    // Persistent Streaming
    public const string PubSubStore = nameof(PubSubStore);
    public const string EventHubCheckpoints = "TheCodeKitchenEventHubCheckpoints";

    public static readonly string[] All =
    [
        Clustering,
        Reminders,
        PubSubStore,
        EventHubCheckpoints
    ];
}