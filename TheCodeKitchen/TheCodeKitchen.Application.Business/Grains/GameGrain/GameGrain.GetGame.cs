using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public Task<Result<GetGameResponse>> GetGame()
    {
        Result<GetGameResponse> result = state.RecordExists
            ? mapper.Map<GetGameResponse>(state.State) with { Paused = _nextMomentTimer is null }
            : new NotFoundError($"The game with id {this.GetPrimaryKey()} does not exist");
        return Task.FromResult(result);
    }
}