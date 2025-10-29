namespace TheCodeKitchen.Infrastructure.AzureSignalR.Constants;

public static class GroupConstants
{
    public static string GetGameGroup(Guid gameId) => $"game-{gameId}";
    public static string GetKitchenGroup(Guid kitchenId) => $"kitchen-{kitchenId}";
}