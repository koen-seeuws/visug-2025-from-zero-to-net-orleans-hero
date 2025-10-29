using TheCodeKitchen.Application.Contracts.Requests.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> UpdateGame(UpdateGameRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");

        var running = _nextMomentTimer is not null;

        if (running)
            await PauseOrUnpauseGame();
        
        state.State.TimePerMoment = request.TimePerMoment;
        state.State.SpeedModifier = request.SpeedModifier;
        state.State.MinimumTimeBetweenOrders = request.MinimumTimeBetweenOrders;
        state.State.MaximumTimeBetweenOrders = request.MaximumTimeBetweenOrders;
        state.State.MinimumItemsPerOrder = request.MinimumItemsPerOrder;
        state.State.MaximumItemsPerOrder = request.MaximumItemsPerOrder;
        state.State.OrderSpeedModifier = request.OrderSpeedModifier;
        state.State.Temperature = request.Temperature;

        await state.WriteStateAsync();
        
        if (running)
            await PauseOrUnpauseGame();

        return TheCodeKitchenUnit.Value;
    }
}