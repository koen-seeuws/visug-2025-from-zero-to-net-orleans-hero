using Microsoft.AspNetCore.SignalR;
using TheCodeKitchen.Infrastructure.AzureSignalR.Constants;

namespace TheCodeKitchen.Presentation.ManagementUI.Hubs;

public sealed class GameHub : Hub
{
    private const string GameIdQueryParameterKey = "gameId";

    public override async Task OnConnectedAsync()
    {
        var gameIdQueryParameterValue = Context.GetHttpContext()?.Request.Query[GameIdQueryParameterKey];

        if (string.IsNullOrWhiteSpace(gameIdQueryParameterValue))
        {
            throw new HubException($"{GameIdQueryParameterKey} query parameter is required");
        }
        
        if (!Guid.TryParse(gameIdQueryParameterValue, out var gameId))
        {
            throw new HubException($"{GameIdQueryParameterKey} query parameter must be a valid GUID");
        }

        Context.Items[GameIdQueryParameterKey] = gameId;
        
        var gameGroup = GroupConstants.GetGameGroup(gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameGroup);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(GameIdQueryParameterKey, out var gameIdObj) &&
            gameIdObj is Guid gameId)
        {
            var gameGroup = GroupConstants.GetGameGroup(gameId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameGroup);
        }

        await base.OnDisconnectedAsync(exception);
    }
}