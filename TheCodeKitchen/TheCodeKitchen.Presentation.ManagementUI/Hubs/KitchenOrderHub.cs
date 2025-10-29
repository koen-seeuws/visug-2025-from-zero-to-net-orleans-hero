using Microsoft.AspNetCore.SignalR;
using TheCodeKitchen.Infrastructure.AzureSignalR.Constants;

namespace TheCodeKitchen.Presentation.ManagementUI.Hubs;

public sealed class KitchenOrderHub : Hub
{
    private const string KitchenIdQueryParameterKey = "kitchenId";

    public override async Task OnConnectedAsync()
    {
        var kitchenIdQueryParameterValue = Context.GetHttpContext()?.Request.Query[KitchenIdQueryParameterKey];

        if (string.IsNullOrWhiteSpace(kitchenIdQueryParameterValue))
        {
            throw new HubException($"{KitchenIdQueryParameterKey} query parameter is required");
        }

        if (!Guid.TryParse(kitchenIdQueryParameterValue, out var kitchenId))
        {
            throw new HubException($"{KitchenIdQueryParameterKey} query parameter must be a valid GUID");
        }

        Context.Items[KitchenIdQueryParameterKey] = kitchenId;

        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        await Groups.AddToGroupAsync(Context.ConnectionId, kitchenGroup);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (Context.Items.TryGetValue(KitchenIdQueryParameterKey, out var kitchenIdObj) &&
            kitchenIdObj is Guid kitchenId)
        {
            var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, kitchenGroup);
        }

        await base.OnDisconnectedAsync(exception);
    }
}