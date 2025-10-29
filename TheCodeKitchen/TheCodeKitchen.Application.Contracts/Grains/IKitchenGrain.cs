using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Cook;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Order;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface IKitchenGrain : IGrainWithGuidKey
{
    Task<Result<CreateKitchenResponse>> Initialize(CreateKitchenRequest request, int count);
    Task<Result<TheCodeKitchenUnit>> InitializeEquipment();
    Task<Result<GetKitchenResponse>> GetKitchen();
    Task<Result<IEnumerable<GetCookResponse>>> GetCooks(GetCookRequest request);
    Task<Result<CreateCookResponse>> CreateCook(CreateCookRequest request);
    Task<Result<JoinKitchenResponse>> JoinKitchen(JoinKitchenRequest request);
    Task<Result<TheCodeKitchenUnit>> CloseOrder(CloseOrderRequest request);
    Task<Result<IEnumerable<GetOpenOrderResponse>>> GetOpenOrders();
    Task<Result<TheCodeKitchenUnit>> DeliverMessage(DeliverMessageRequest request);

    [ResponseTimeout("00:05:00")]
    Task<Result<TheCodeKitchenUnit>> ResetKitchen();
}