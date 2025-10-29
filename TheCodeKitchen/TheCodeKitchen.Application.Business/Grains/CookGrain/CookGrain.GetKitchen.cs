using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<GetKitchenResponse>> GetKitchen()
    {
        if (!state.RecordExists)
            return new NotFoundError(
                $"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");
        
        var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(state.State.Kitchen);
        var getKitchenResult = await kitchenGrain.GetKitchen();

        if (!getKitchenResult.Succeeded)
            return getKitchenResult.Error;

        return getKitchenResult.Value;
    }
}