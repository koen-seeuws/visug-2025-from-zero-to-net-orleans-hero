using TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Response.KitchenOrder;

namespace TheCodeKitchen.Application.Contracts.Grains;

//Key exists of OrderNumber and KitchenId
public interface IKitchenOrderGrain : IGrainWithIntegerCompoundKey
{
    Task<Result<CreateKitchenOrderResponse>> Initialize(CreateKitchenOrderRequest request);
    Task<Result<GetKitchenOrderResponse>> GetKitchenOrder();
    Task<Result<TheCodeKitchenUnit>> DeliverFood(DeliverFoodRequest request);
    Task<Result<TheCodeKitchenUnit>> Complete();
    Task<Result<TheCodeKitchenUnit>> Cancel();
}