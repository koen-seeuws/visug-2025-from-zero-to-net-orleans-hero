using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;

namespace TheCodeKitchen.Infrastructure.AzureSignalR.Services;

public sealed class RealTimeGameService(HubContextProvider hubContextProvider) : IRealTimeGameService
{
    public async Task SendKitchenCreatedEvent(Guid gameId, KitchenCreatedEvent @event)
    {
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        var gameHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameHub);
        await gameHubContext.Clients.Group(gameGroup).SendAsync(nameof(KitchenCreatedEvent), @event);
    }

    public async Task SendCookJoinedEvent(Guid gameId, CookJoinedEvent @event)
    {
        var gameHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameHub);
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        await gameHubContext.Clients.Group(gameGroup).SendAsync(nameof(CookJoinedEvent), @event);
    }

    public async Task SendGamePausedOrResumedEvent(Guid gameId, GamePausedOrResumedEvent @event)
    {
        var gameHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameHub);
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        await gameHubContext.Clients.Group(gameGroup).SendAsync(nameof(GamePausedOrResumedEvent), @event);
    }

    public async Task SendMomentPassedEvent(Guid gameId, MomentPassedEvent @event)
    {
        var gameHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameHub);
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        await gameHubContext.Clients.Group(gameGroup).SendAsync(nameof(MomentPassedEvent), @event);
    }

    public async Task SendGameResetEvent(Guid gameId, GameResetEvent @event)
    {
        var gameHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.GameHub);
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        await gameHubContext.Clients.Group(gameGroup).SendAsync(nameof(GameResetEvent), @event);
    }
}