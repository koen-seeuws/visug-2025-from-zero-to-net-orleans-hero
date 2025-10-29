namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ResumeGame()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        if (state.State.Started is null)
            return new GameNotStartedError($"The game with id {this.GetPrimaryKey()} has not yet started");

        if (_nextMomentTimer is not null) 
            return TheCodeKitchenUnit.Value;
        
        _nextMomentDelay = TimeSpan.FromSeconds(1) / state.State.SpeedModifier;
            
        _nextMomentTimer = this.RegisterGrainTimer(NextMoment, TimeSpan.Zero, _nextMomentDelay.Value);
            
        await this.RegisterOrUpdateReminder(nameof(ResumeGame), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        
        return TheCodeKitchenUnit.Value;
    }
}