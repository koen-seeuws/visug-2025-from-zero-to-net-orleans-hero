using TheCodeKitchen.Application.Contracts.Requests.Order;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> GenerateOrder()
    {
        var orderNumber = state.State.OrderNumbers.DefaultIfEmpty(0).Max() + 1;

        var game = state.State.Id;
        var orderGrain = GrainFactory.GetGrain<IOrderGrain>(orderNumber, game.ToString());

        state.State.OrderNumbers.Add(orderNumber); // Already add number here in case something went wrong with persistence
        await state.WriteStateAsync();
        
        var generateOrderRequest =
            new GenerateOrderRequest(state.State.MinimumItemsPerOrder, state.State.MaximumItemsPerOrder);
        var generateOrderResult = await orderGrain.GenerateOrder(generateOrderRequest);

        if (!generateOrderResult.Succeeded)
            return generateOrderResult.Error;

        var timeBetweenOrders = generateOrderResult.Value.MinimumTimeToPrepare;

        if (timeBetweenOrders < state.State.MinimumTimeBetweenOrders)
            timeBetweenOrders = state.State.MinimumTimeBetweenOrders;
        
        if (timeBetweenOrders > state.State.MaximumTimeBetweenOrders)
            timeBetweenOrders = state.State.MaximumTimeBetweenOrders;

        _timeUntilNewOrder = timeBetweenOrders / state.State.OrderSpeedModifier;

        return TheCodeKitchenUnit.Value;
    }
}