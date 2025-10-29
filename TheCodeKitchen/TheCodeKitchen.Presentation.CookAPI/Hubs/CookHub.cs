using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TheCodeKitchen.Infrastructure.AzureSignalR.Constants;
using TheCodeKitchen.Infrastructure.Security.Extensions;

namespace TheCodeKitchen.Presentation.API.Cook.Hubs;

[Authorize]
public sealed class CookHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Kitchen Group
        var kitchenId = Context.User?.GetKitchenId() ?? throw new UnauthorizedAccessException();
        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        await Groups.AddToGroupAsync(Context.ConnectionId, kitchenGroup);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //Kitchen Group
        var kitchenId = Context.User?.GetKitchenId() ?? throw new UnauthorizedAccessException();
        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, kitchenGroup);

        await base.OnDisconnectedAsync(exception);
    }
}