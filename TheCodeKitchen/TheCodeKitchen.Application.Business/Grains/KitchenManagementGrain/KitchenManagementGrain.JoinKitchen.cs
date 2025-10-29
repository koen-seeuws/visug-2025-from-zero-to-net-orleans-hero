using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenManagementGrain;

public sealed partial class KitchenManagementGrain
{
    public async Task<Result<JoinKitchenResponse>> JoinKitchen(JoinKitchenRequest request)
    {
        var retrieved = state.State.KitchenCodes.TryGetValue(request.KitchenCode, out var kitchenId);

        if (!retrieved)
            return new NotFoundError($"The kitchen code {request.KitchenCode} does not exist");

        var kitchenGrain = GrainFactory.GetGrain<IKitchenGrain>(kitchenId);
        var result = await kitchenGrain.JoinKitchen(request);

        return result;
    }
}