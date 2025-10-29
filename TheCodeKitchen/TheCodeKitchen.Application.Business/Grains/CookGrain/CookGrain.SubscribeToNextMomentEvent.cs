using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    private async Task SubscribeToNextMomentEvent()
    {
        if (streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle is not null)
        {
            streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle =
                await streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle.ResumeAsync(OnNextMomentEvent);
            await streamSubscriptionHandles.WriteStateAsync();
        }
        else if (state.RecordExists)
        {
            var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
            var stream = streamProvider.GetStream<NextMomentEvent>(nameof(NextMomentEvent), state.State.Game);
            streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle =
                await stream.SubscribeAsync(OnNextMomentEvent);
            await streamSubscriptionHandles.WriteStateAsync();
        }
    }
}