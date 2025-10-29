using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Response.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    public async Task<Result<CreateKitchenOrderResponse>> Initialize(CreateKitchenOrderRequest request)
    {
        var orderNumber = this.GetPrimaryKeyLong();
        var kitchen = Guid.Parse(this.GetPrimaryKeyString().Split('+')[1]);

        if (state.RecordExists)
            return new AlreadyExistsError(
                $"The order with number {orderNumber} has already been initialized in kitchen {kitchen}");

        var requestedFoods = mapper
            .Map<List<KitchenOrderFoodRequest>>(request.RequestedFoods)
            .ToList();
        
        var kitchenOrder = new KitchenOrder(orderNumber, requestedFoods, request.Game, kitchen);
        state.State = kitchenOrder;
        await state.WriteStateAsync();

        await SubscribeToNextMomentEvent();

        var requestedFoodNames = request.RequestedFoods.Select(f => f.Food).ToList();
        var @event = new KitchenOrderCreatedEvent(request.OrderNumber, requestedFoodNames);
        await realTimeKitchenOrderService.SendKitchenOrderCreatedEvent(state.State.Kitchen, @event);

        return mapper.Map<CreateKitchenOrderResponse>(kitchenOrder);
    }
}