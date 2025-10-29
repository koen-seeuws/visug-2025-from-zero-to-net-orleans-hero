using TheCodeKitchen.Application.Business.Extensions;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<TheCodeKitchenUnit>> StartGame()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        if (state.State.Started is not null)
            return new GameAlreadyStartedError($"The game with id {this.GetPrimaryKey()} has already started");
        
        if (state.State.Kitchens.Count is 0)
            return new EmptyError($"The game with id {this.GetPrimaryKey()} has no kitchens");
        
        var initializeKitchenEquipmentTasks = state.State.Kitchens
            .Select(kitchen =>
            {
                var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(kitchen);
                return kitchenGrain.InitializeEquipment();
            });
        
        var initializeKitchenEquipmentResults = await Task.WhenAll(initializeKitchenEquipmentTasks);
        var initializeKitchensResult = initializeKitchenEquipmentResults.Combine();
        
        if(!initializeKitchensResult.Succeeded)
            return initializeKitchensResult.Error;
        
        state.State.Started = DateTimeOffset.UtcNow;

        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}