using TheCodeKitchen.Application.Contracts.Events.GameManagement;

namespace TheCodeKitchen.Infrastructure.AzureSignalR.Services;

public sealed class RealTimeGameManagementService(HubContextProvider hubContextProvider) : IRealTimeGameManagementService
{
    public async Task SendGameCreatedEvent(GameCreatedEvent @event)
    {
        var gameManagementHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameManagementHub);
        await gameManagementHubContext.Clients.All.SendAsync(nameof(GameCreatedEvent), @event);
    }
}