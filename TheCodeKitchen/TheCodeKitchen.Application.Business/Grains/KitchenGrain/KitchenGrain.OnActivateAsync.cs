namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        streamHandles.State ??= new KitchenGrainStreamSubscriptionHandles();
        await SubscribeToNextMomentEvent();
        await SubscribeToOrderGeneratedEvent();
        await SubscribeToKitchenOrderRatingUpdatedEvent();
        await base.OnActivateAsync(cancellationToken);
    }
}