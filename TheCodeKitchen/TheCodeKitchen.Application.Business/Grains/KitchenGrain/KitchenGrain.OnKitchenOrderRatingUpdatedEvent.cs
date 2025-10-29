using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    private Task OnKitchenOrderRatingUpdatedEvent(KitchenOrderRatingUpdatedEvent kitchenOrderRatingUpdatedEvent,
        StreamSequenceToken _)
    {
        state.State.OrderRatings[kitchenOrderRatingUpdatedEvent.Order] = kitchenOrderRatingUpdatedEvent.Rating;
        return Task.CompletedTask;
    }
}