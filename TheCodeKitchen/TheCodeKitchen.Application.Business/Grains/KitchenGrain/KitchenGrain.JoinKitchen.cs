using TheCodeKitchen.Application.Contracts.Events.Kitchen;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<JoinKitchenResponse>> JoinKitchen(JoinKitchenRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The kitchen with id {this.GetPrimaryKey()} has not been initialized");

        var gameGrain = GrainFactory.GetGrain<IGameGrain>(state.State.Game);
        
        var getCooksResult = await GetCooks(new GetCookRequest(request.Username));

        if (!getCooksResult.Succeeded)
            return getCooksResult.Error;

        var foundCook = getCooksResult.Value.FirstOrDefault();
        
        if (foundCook is not null)
            return new JoinKitchenResponse(state.State.Game,
                state.State.Id,
                foundCook.Username,
                foundCook.PasswordHash,
                false
            );
        
        var game = await gameGrain.GetGame();

        if (!game.Succeeded)
            return game.Error;
        
        if (game.Value.Started is not null)
            return new GameAlreadyStartedError(
                $"The game with id {state.State.Game} has already started, you can't join a game that has already started!");

        var createCookResult =
            await CreateCook(new CreateCookRequest(request.Username, request.PasswordHash, state.State.Game, state.State.Id));
        
        if (!createCookResult.Succeeded)
            return createCookResult.Error;
        
        var @event = new CookJoinedEvent(createCookResult.Value.Username, state.State.Id);
        await realTimeGameService.SendCookJoinedEvent(state.State.Game, @event);
        
        return new JoinKitchenResponse(
            state.State.Game,
            state.State.Id,
            createCookResult.Value.Username,
            request.PasswordHash,
            true
        );
    }
}