using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ResetGame()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        if (state.State.Started is null)
            return new GameNotStartedError($"The game with id {this.GetPrimaryKey()} has not yet started");
        
        if(_nextMomentTimer is not null)
            return new GameNotPausedError($"The game with id {this.GetPrimaryKey()} must be paused before it can be reset");
        
        var resetKitchenTasks = state.State.Kitchens
            .Select(async kitchen =>
            {
                var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(kitchen);
                var resetKitchenResult = await kitchenGrain.ResetKitchen();
                return resetKitchenResult;
            });

        var cancelOrdersTasks = state.State.OrderNumbers
            .Select(async order =>
            {
                var orderGrain = GrainFactory.GetGrain<IOrderGrain>(order, state.State.Id.ToString());
                var cancelOrderResult = await orderGrain.Cancel();
                return cancelOrderResult;
            });

        var allResetTasks = resetKitchenTasks.Concat(cancelOrdersTasks);

        var resetResults = await Task.WhenAll(allResetTasks);

        var resetResult = resetResults.Combine();

        if (!resetResult.Succeeded)
            return resetResult.Error;

        state.State.OrderNumbers.Clear();
        state.State.TimePassed = TimeSpan.Zero;
        _timeUntilNewOrder = TimeSpan.Zero;
        await state.WriteStateAsync();
        
        var gameResetEvent = new GameResetEvent();
        await realTimeGameService.SendGameResetEvent(state.State.Id, gameResetEvent);

        return TheCodeKitchenUnit.Value;
    }
}