using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

namespace TheCodeKitchen.Application.Contracts.Realtime;

public interface IRealTimeKitchenOrderService
{
    Task SendKitchenOrderCreatedEvent(Guid kitchenId, KitchenOrderCreatedEvent @event);
    Task SendKitchenOrderFoodDeliveredEvent(Guid kitchenId, KitchenOrderFoodDeliveredEvent @event);
    Task SendKitchenOrderCompletedEvent(Guid kitchenId, KitchenOrderCompletedEvent @event);
}