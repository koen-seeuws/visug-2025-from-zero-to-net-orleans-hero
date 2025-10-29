using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task<Result<IEnumerable<GetKitchenResponse>>> GetKitchens()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The game with id {this.GetPrimaryKey()} has not been initialized");
        
        var tasks = state.State.Kitchens.Select(async id =>
        {
            var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(id);
            var result = await kitchenGrain.GetKitchen();
            return result;
        });

        var results = await Task.WhenAll(tasks);

        return results.Combine();
    }
}