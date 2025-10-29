using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Game;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface IGameGrain : IGrainWithGuidKey
{
    Task<Result<CreateKitchenResponse>> CreateKitchen(CreateKitchenRequest request);
    Task<Result<GetGameResponse>> GetGame();
    Task<Result<IEnumerable<GetKitchenResponse>>> GetKitchens();
    Task<Result<CreateGameResponse>> Initialize(CreateGameRequest request, int count);
    Task<Result<TheCodeKitchenUnit>> GenerateOrder();
    Task<Result<TheCodeKitchenUnit>> NextMoment();
    Task<Result<TheCodeKitchenUnit>> PauseGame();
    Task<Result<PauseOrResumeGameResponse>> PauseOrUnpauseGame();
    Task<Result<TheCodeKitchenUnit>> ResumeGame();
    Task<Result<TheCodeKitchenUnit>> StartGame();
    Task<Result<TheCodeKitchenUnit>> UpdateGame(UpdateGameRequest request);
    
    [ResponseTimeout("00:10:00")]
    Task<Result<TheCodeKitchenUnit>> ResetGame();
}