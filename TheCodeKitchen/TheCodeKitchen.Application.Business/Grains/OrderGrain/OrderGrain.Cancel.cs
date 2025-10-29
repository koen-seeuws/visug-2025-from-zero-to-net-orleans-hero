namespace TheCodeKitchen.Application.Business.Grains.OrderGrain;

public sealed partial class OrderGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Cancel()
    {
        await state.ClearStateAsync();
        DeactivateOnIdle();
        return TheCodeKitchenUnit.Value;
    }
}