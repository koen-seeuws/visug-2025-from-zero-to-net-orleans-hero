using TheCodeKitchen.Application.Contracts.Events.GameManagement;
using TheCodeKitchen.Application.Contracts.Requests.Game;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameManagementGrain;

public sealed partial class GameManagementGrain
{
    public async Task<Result<CreateGameResponse>> CreateGame(CreateGameRequest request)
    {
        var id = Guid.CreateVersion7();
        state.State.Games.Add(id);
        await state.WriteStateAsync();
        
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(id);
        var result = await gameGrain.Initialize(request, state.State.Games.Count);

        if (!result.Succeeded)
            return result.Error;

        var game = result.Value;
        
        var @event = new GameCreatedEvent(game.Id, game.Name, game.SpeedModifier, game.Temperature,null, true);
        await realTimeGameManagementService.SendGameCreatedEvent(@event);
        
        return result;
    }
}