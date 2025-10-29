using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Response.Cook;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<CreateCookResponse>> CreateCook(CreateCookRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");

        var cookResult = await GetCooks(new GetCookRequest(request.Username));
        if (!cookResult.Succeeded)
            return cookResult.Error;
        
        var cook = cookResult.Value.FirstOrDefault();
        if(cook is not null)
            return new AlreadyExistsError($"The cook with username {request.Username} already exists");
        
        var cookGrain = GrainFactory.GetGrain<ICookGrain>(state.State.Id, request.Username);
        var createCookResult = await cookGrain.Initialize(request);
        
        if(!createCookResult.Succeeded)
            return createCookResult.Error;
        
        state.State.Cooks.Add(createCookResult.Value.Username);
        await state.WriteStateAsync();
        
        return createCookResult;
    }
}