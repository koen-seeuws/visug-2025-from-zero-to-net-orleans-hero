using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    private async Task SubscribeToKitchenOrderRatingUpdatedEvent()
    {
        if (streamHandles.State.KitchenOrderRatingUpdatedStreamSubscriptionHandle is not null)
        {
            streamHandles.State.KitchenOrderRatingUpdatedStreamSubscriptionHandle =
                await streamHandles.State.KitchenOrderRatingUpdatedStreamSubscriptionHandle.ResumeAsync(
                    OnKitchenOrderRatingUpdatedEvent);
            await streamHandles.WriteStateAsync();
        }
        else if (state.RecordExists)
        {
            var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
            var stream =
                streamProvider.GetStream<KitchenOrderRatingUpdatedEvent>(nameof(KitchenOrderRatingUpdatedEvent),
                    state.State.Id);
            streamHandles.State.KitchenOrderRatingUpdatedStreamSubscriptionHandle =
                await stream.SubscribeAsync(OnKitchenOrderRatingUpdatedEvent);
            await streamHandles.WriteStateAsync();
        }
    }
}