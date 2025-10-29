using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    private async Task SubscribeToOrderGeneratedEvent()
    {
        if (streamHandles.State.OrderGeneratedStreamSubscriptionHandle is not null)
        {
            streamHandles.State.OrderGeneratedStreamSubscriptionHandle =
                await streamHandles.State.OrderGeneratedStreamSubscriptionHandle.ResumeAsync(OnOrderGeneratedEvent);
            await streamHandles.WriteStateAsync();
        }
        else if (state.RecordExists)
        {
            var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
            var stream = streamProvider.GetStream<OrderGeneratedEvent>(nameof(OrderGeneratedEvent), state.State.Game);
            streamHandles.State.OrderGeneratedStreamSubscriptionHandle = await stream.SubscribeAsync(OnOrderGeneratedEvent);
            await streamHandles.WriteStateAsync();
        }
    }
}