namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        streamSubscriptionHandles.State ??= new EquipmentGrainStreamSubscriptionHandles();
        
        if (streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle is not null)
        {
            // Resume subscription to NextMomentEvent
            streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle =
                await streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle.ResumeAsync(OnNextMomentEvent);
            await streamSubscriptionHandles.WriteStateAsync(cancellationToken);
        }
        
        await base.OnActivateAsync(cancellationToken);
    }
}