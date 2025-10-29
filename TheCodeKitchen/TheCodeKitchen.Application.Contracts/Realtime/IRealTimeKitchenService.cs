using TheCodeKitchen.Application.Contracts.Events.Kitchen;

namespace TheCodeKitchen.Application.Contracts.Realtime;

public interface IRealTimeKitchenService
{
    Task SendKitchenRatingUpdatedEvent(Guid kitchenId, KitchenRatingUpdatedEvent @event);
    Task SendMessageDeliveredEvent(Guid kitchenId, MessageDeliveredEvent @event);
    Task SendKitchenResetEvent(Guid kitchenId, KitchenResetEvent @event);
}