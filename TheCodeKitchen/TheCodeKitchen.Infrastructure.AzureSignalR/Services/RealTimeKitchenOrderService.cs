using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

namespace TheCodeKitchen.Infrastructure.AzureSignalR.Services;

public sealed class RealTimeKitchenOrderService(HubContextProvider hubContextProvider) : IRealTimeKitchenOrderService
{
    public async Task SendKitchenOrderCreatedEvent(Guid kitchenId, KitchenOrderCreatedEvent @event)
    {
        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        
        var cookHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.CookHub); // Players (Cook API)
        await cookHubContext.Clients.Group(kitchenGroup).SendAsync(nameof(KitchenOrderCreatedEvent), @event);
        
        var kitchenHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.KitchenOrderHub); // UI
        await kitchenHubContext.Clients.Group(kitchenGroup).SendAsync(nameof(KitchenOrderCreatedEvent), @event);
    }

    public async Task SendKitchenOrderFoodDeliveredEvent(Guid kitchenId, KitchenOrderFoodDeliveredEvent @event)
    {
        var kitchenHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.KitchenOrderHub);
        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        await kitchenHubContext.Clients.Group(kitchenGroup).SendAsync(nameof(KitchenOrderFoodDeliveredEvent), @event);
    }

    public async Task SendKitchenOrderCompletedEvent(Guid kitchenId, KitchenOrderCompletedEvent @event)
    {
        var kitchenHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.KitchenOrderHub);
        var kitchenGroup = GroupConstants.GetKitchenGroup(kitchenId);
        await kitchenHubContext.Clients.Group(kitchenGroup).SendAsync(nameof(KitchenOrderCompletedEvent), @event);
    }
}