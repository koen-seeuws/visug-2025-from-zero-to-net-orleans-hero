namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> ThrowFoodAway()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");

        if (state.State.Food is null)
            return new NotHoldingFoodError(
                $"The cook with name {state.State.Username} is not holding any food");

        state.State.Food = null;
        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}