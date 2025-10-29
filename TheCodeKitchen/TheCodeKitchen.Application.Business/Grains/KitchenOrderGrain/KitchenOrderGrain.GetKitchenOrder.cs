using TheCodeKitchen.Application.Contracts.Response.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed partial class KitchenOrderGrain
{
    public Task<Result<GetKitchenOrderResponse>> GetKitchenOrder()
    {
        var orderNumber = this.GetPrimaryKeyLong();
        var kitchen = Guid.Parse(this.GetPrimaryKeyString().Split('+')[1]);
        
        Result<GetKitchenOrderResponse> result = state.RecordExists
            ? mapper.Map<GetKitchenOrderResponse>(state.State) 
            : new NotFoundError($"The order with number {orderNumber} does not exist in kitchen {kitchen}");
        
        return Task.FromResult(result);
    }
}