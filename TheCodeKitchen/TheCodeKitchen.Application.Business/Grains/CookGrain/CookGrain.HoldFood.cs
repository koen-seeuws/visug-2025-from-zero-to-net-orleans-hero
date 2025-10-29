using TheCodeKitchen.Application.Contracts.Requests.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> HoldFood(HoldFoodRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError($"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");

        if (state.State.Food is not null)
            return new AlreadyHoldingFoodError(
                $"The cook with name {state.State.Username} is already holding food");
        
        state.State.Food = mapper.Map<Food>(request.Food);
        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}