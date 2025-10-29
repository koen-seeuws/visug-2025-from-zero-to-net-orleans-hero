using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    private async Task SubscribeToNextMomentEvent()
    {
        if (streamHandles.State.NextMomentStreamSubscriptionHandle is not null)
        {
            streamHandles.State.NextMomentStreamSubscriptionHandle =
                await streamHandles.State.NextMomentStreamSubscriptionHandle.ResumeAsync(OnNextMomentEvent);
            await streamHandles.WriteStateAsync();
        }
        else if (state.RecordExists)
        {
            var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
            var stream = streamProvider.GetStream<NextMomentEvent>(nameof(NextMomentEvent), state.State.Game);
            streamHandles.State.NextMomentStreamSubscriptionHandle = await stream.SubscribeAsync(OnNextMomentEvent);
            await streamHandles.WriteStateAsync();
        }
    }
}