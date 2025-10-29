using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    private async Task<Result<TheCodeKitchenUnit>> UpdateKitchenRating()
    {
        var requestedFoodRating = state.State.RequestedFoods
            .Select(r => r.Rating)
            .DefaultIfEmpty(1.0)
            .Average();

        var deliveredFoodRating = state.State.DeliveredFoods
            .Select(r => r.Rating)
            .DefaultIfEmpty(0.0)
            .Average();

        double[] ratings = [requestedFoodRating, deliveredFoodRating, state.State.CompletenessRating];

        state.State.TotalRating = ratings.Average();
        
        var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
        var stream = streamProvider.GetStream<KitchenOrderRatingUpdatedEvent>(
            nameof(KitchenOrderRatingUpdatedEvent), state.State.Kitchen);
        var kitchenOrderRatingUpdatedEvent = new KitchenOrderRatingUpdatedEvent(state.State.Number, state.State.TotalRating);
        await stream.OnNextAsync(kitchenOrderRatingUpdatedEvent);

        return TheCodeKitchenUnit.Value;
    }
}