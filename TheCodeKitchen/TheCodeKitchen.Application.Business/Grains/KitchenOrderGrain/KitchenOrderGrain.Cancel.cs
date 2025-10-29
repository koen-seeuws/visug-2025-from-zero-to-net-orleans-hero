namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Cancel()
    {
        if (!state.RecordExists)
        {
            var orderNumber = this.GetPrimaryKeyLong();
            var kitchenId = Guid.Parse(this.GetPrimaryKeyString().Split('+')[1]);
            return new NotFoundError($"The order with number {orderNumber} does not exist in kitchen {kitchenId}");
        }
        
        if (streamHandles.State.NextMomentStreamSubscriptionHandle is not null)
        {
            await streamHandles.State.NextMomentStreamSubscriptionHandle.UnsubscribeAsync();
            await streamHandles.ClearStateAsync();
        }

        await state.ClearStateAsync();
        
        DeactivateOnIdle();

        return TheCodeKitchenUnit.Value;
    }
}