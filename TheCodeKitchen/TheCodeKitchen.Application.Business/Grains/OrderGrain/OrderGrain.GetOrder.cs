using TheCodeKitchen.Application.Contracts.Response.Order;

namespace TheCodeKitchen.Application.Business.Grains.OrderGrain;

public sealed partial class OrderGrain
{
    public async Task<Result<GetOrderResponse>> GetOrder()
    {
        if (state.RecordExists) return mapper.Map<GetOrderResponse>(state.State);
        
        var orderNumber = this.GetPrimaryKeyLong();
        var game = Guid.Parse(this.GetPrimaryKeyString().Split('+')[1]);

        return new NotFoundError(
            $"The order with number {orderNumber} and game {game} was not found");
    }
}