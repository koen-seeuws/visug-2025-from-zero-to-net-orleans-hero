using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface IGameManagementGrain : IGrainWithGuidKey
{
    Task<Result<CreateGameResponse>> CreateGame(CreateGameRequest request);
    Task<Result<IEnumerable<GetGameResponse>>> GetGames();
}