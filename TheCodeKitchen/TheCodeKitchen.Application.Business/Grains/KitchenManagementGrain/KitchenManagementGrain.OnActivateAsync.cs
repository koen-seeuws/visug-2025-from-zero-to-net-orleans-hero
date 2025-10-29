namespace TheCodeKitchen.Application.Business.Grains.KitchenManagementGrain;

public sealed partial class KitchenManagementGrain
{
    public sealed override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!state.RecordExists)
        {
            state.State = new KitchenManagementState();
            await state.WriteStateAsync(cancellationToken);
        }

        await base.OnActivateAsync(cancellationToken);
    }
}