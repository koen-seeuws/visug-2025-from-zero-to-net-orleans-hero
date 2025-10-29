using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Response.Cook;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface ICookGrain : IGrainWithGuidCompoundKey
{
    Task<Result<CreateCookResponse>> Initialize(CreateCookRequest request);
    Task<Result<GetCookResponse>> GetCook();
    Task<Result<GetKitchenResponse>> GetKitchen();
    Task<Result<TheCodeKitchenUnit>> HoldFood(HoldFoodRequest request);
    Task<Result<ReleaseFoodResponse>> ReleaseFood();
    Task<Result<TheCodeKitchenUnit>> ThrowFoodAway();
    Task<Result<TheCodeKitchenUnit>> ReceiveMessage(ReceiveMessageRequest request);
    Task<Result<IEnumerable<ReadMessageResponse>>> ReadMessages();
    Task<Result<TheCodeKitchenUnit>> ConfirmMessage(ConfirmMessageRequest request);
    Task<Result<TheCodeKitchenUnit>> SetTimer(SetTimerRequest request);
    Task<Result<IEnumerable<GetTimerResponse>>> GetTimers();
    Task<Result<TheCodeKitchenUnit>> StopTimer(StopTimerRequest request);
    Task<Result<TheCodeKitchenUnit>> Reset();
}