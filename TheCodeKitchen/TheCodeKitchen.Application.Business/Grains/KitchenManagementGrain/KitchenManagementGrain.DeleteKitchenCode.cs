namespace TheCodeKitchen.Application.Business.Grains.KitchenManagementGrain;

public sealed partial class KitchenManagementGrain
{
    public async Task<Result<TheCodeKitchenUnit>> DeleteKitchenCode(string code)
    {
        var removed = state.State.KitchenCodes.Remove(code);

        if (!removed)
            return new NotFoundError($"The kitchen code {code} does not exist");

        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}