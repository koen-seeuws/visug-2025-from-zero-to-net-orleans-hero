namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> PauseGame()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");

        if (state.State.Started is null)
            return new GameNotStartedError($"The game with id {this.GetPrimaryKey()} has not yet started");
        
        _nextMomentTimer?.Dispose();
        _nextMomentTimer = null;

        var resumeGameReminder = await this.GetReminder(nameof(ResumeGame));
        if (resumeGameReminder is not null)
            await this.UnregisterReminder(resumeGameReminder);

        return TheCodeKitchenUnit.Value;
    }
}