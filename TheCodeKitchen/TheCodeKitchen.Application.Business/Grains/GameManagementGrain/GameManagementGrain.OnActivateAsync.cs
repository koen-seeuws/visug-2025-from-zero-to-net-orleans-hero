namespace TheCodeKitchen.Application.Business.Grains.GameManagementGrain;

public sealed partial class GameManagementGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!state.RecordExists)
        {
            state.State = new GameManagementState();
            await state.WriteStateAsync(cancellationToken);
        }

        await base.OnActivateAsync(cancellationToken);
    }
}