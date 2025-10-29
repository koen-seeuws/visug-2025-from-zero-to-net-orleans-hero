using TheCodeKitchen.Application.Contracts.Requests.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<TheCodeKitchenUnit>> CloseOrder(CloseOrderRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The kitchen with id {this.GetPrimaryKey()} does not exist");
        
        state.State.OpenOrders.Remove(request.OrderNumber);
        await state.WriteStateAsync();
        
        return TheCodeKitchenUnit.Value;
    }
}