using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    private async Task OnOrderGeneratedEvent(OrderGeneratedEvent orderGeneratedEvent, StreamSequenceToken _)
    {
        var createKitchenOrderRequest = new CreateKitchenOrderRequest(state.State.Game, state.State.Id,
            orderGeneratedEvent.Number, orderGeneratedEvent.RequestedFoods);

        var kitchenOrderGrain = GrainFactory.GetGrain<IKitchenOrderGrain>(orderGeneratedEvent.Number, state.State.Id.ToString());
        
        var createKitchenOrderResult = await kitchenOrderGrain.Initialize(createKitchenOrderRequest);

        if (!createKitchenOrderResult.Succeeded)
            return;

        state.State.Orders.Add(orderGeneratedEvent.Number);
        state.State.OpenOrders.Add(orderGeneratedEvent.Number);
        await state.WriteStateAsync();
    }
}