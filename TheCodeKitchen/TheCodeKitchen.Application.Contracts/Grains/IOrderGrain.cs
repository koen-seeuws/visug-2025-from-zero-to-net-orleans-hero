using TheCodeKitchen.Application.Contracts.Requests.Order;
using TheCodeKitchen.Application.Contracts.Response.Order;

namespace TheCodeKitchen.Application.Contracts.Grains;

//Key exists of OrderNumber and GameId
public interface IOrderGrain : IGrainWithIntegerCompoundKey
{
    Task<Result<GenerateOrderResponse>> GenerateOrder(GenerateOrderRequest request);
    Task<Result<GetOrderResponse>> GetOrder();
    Task<Result<TheCodeKitchenUnit>> Cancel();
}