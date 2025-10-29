using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Response.Order;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<IEnumerable<GetOpenOrderResponse>>> GetOpenOrders()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The kitchen with id {this.GetPrimaryKey()} does not exist");

        var tasks = state.State.OpenOrders.Select(async number =>
        {
            var kitchenOrderGrain = GrainFactory.GetGrain<IKitchenOrderGrain>(number, state.State.Id.ToString());
            var result = await kitchenOrderGrain.GetKitchenOrder();
            return result;
        });

        var results = await Task.WhenAll(tasks);
        var orders = results.Combine();
        
        if (!orders.Succeeded)
            return orders.Error;

        var openOrders = orders.Value
            .Select(o =>
            {
                var requestedFoods = o.RequestedFoods
                    .Select(f => f.Food)
                    .ToList();
                
                var deliveredFoods = o.DeliveredFoods
                    .Select(f => f.Food.Name)
                    .ToList();
                
                return new GetOpenOrderResponse(o.Number, requestedFoods, deliveredFoods);
            })
            .ToList();

        return openOrders;
    }
}