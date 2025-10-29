using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<CreateKitchenResponse>> CreateKitchen(CreateKitchenRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        if(state.State.Started is not null)
            return new GameAlreadyStartedError($"The game with id {this.GetPrimaryKey()} has already started, you can't add any new kitchens");

        var id = Guid.CreateVersion7();
        state.State.Kitchens.Add(id);
        await state.WriteStateAsync();
        
        var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(id);
        var result = await kitchenGrain.Initialize(request, state.State.Kitchens.Count);

        if (!result.Succeeded)
            return result.Error;

        var kitchen = result.Value;
        
        var @event = new KitchenCreatedEvent(kitchen.Id, kitchen.Name, kitchen.Code);
        await realTimeGameService.SendKitchenCreatedEvent(state.State.Id, @event);
        
        return result.Value;
    }
}