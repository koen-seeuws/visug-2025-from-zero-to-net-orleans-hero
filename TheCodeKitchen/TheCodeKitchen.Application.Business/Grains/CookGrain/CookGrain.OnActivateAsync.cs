namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        streamSubscriptionHandles.State ??= new EquipmentGrainStreamSubscriptionHandles();
        await SubscribeToNextMomentEvent();
        await base.OnActivateAsync(cancellationToken);
    }
}