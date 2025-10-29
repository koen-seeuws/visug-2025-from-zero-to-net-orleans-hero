using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Complete()
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
            streamHandles.State.NextMomentStreamSubscriptionHandle = null;
            await streamHandles.WriteStateAsync();
        }

        var kitchen = GrainFactory.GetGrain<IKitchenGrain>(state.State.Kitchen);
        var closeOrderRequest = new CloseOrderRequest(state.State.Number);
        var closeOrderResult = await kitchen.CloseOrder(closeOrderRequest);

        if (!closeOrderResult.Succeeded)
            return closeOrderResult.Error;

        state.State.Completed = true;

        await state.WriteStateAsync();

        // Update kitchen rating
        await UpdateKitchenRating();
        
        var @event = new KitchenOrderCompletedEvent(state.State.Number);
        await realTimeKitchenOrderService.SendKitchenOrderCompletedEvent(@state.State.Kitchen, @event);

        DeactivateOnIdle();

        return TheCodeKitchenUnit.Value;
    }
}