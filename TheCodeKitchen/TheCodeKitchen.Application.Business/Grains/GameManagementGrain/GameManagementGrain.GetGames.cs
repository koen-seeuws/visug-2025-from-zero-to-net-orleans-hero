using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameManagementGrain;

public sealed partial class GameManagementGrain
{
    public async Task<Result<IEnumerable<GetGameResponse>>> GetGames()
    {
        var tasks = state.State.Games.Select(async id =>
        {
            var gameGrain = GrainFactory.GetGrain<IGameGrain>(id);
            var result = await gameGrain.GetGame();
            return result;
        });

        var results = await Task.WhenAll(tasks);

        return results.Combine();
    }
}