using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Response.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<PauseOrResumeGameResponse>> PauseOrUnpauseGame()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        var paused = _nextMomentTimer is null;
        
        var result = paused ? await ResumeGame() : await PauseGame();

        if (!result.Succeeded)
            return result.Error;
        
        paused = _nextMomentTimer is null;
        
        var @event = new GamePausedOrResumedEvent(paused);
        await realTimeGameService.SendGamePausedOrResumedEvent(state.State.Id, @event);

        return new PauseOrResumeGameResponse(paused);
    }
}