using TheCodeKitchen.Application.Contracts.Models.Food;
using TheCodeKitchen.Application.Contracts.Response.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<ReleaseFoodResponse>> ReleaseFood()
    {
        if (!state.RecordExists)
            return new NotFoundError($"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");

        if (state.State.Food is null)
            return new NotHoldingFoodError(
                $"The cook with name {state.State.Username} is not holding any food");

        var food = mapper.Map<FoodDto>(state.State.Food);

        state.State.Food = null;
        await state.WriteStateAsync();

        return new ReleaseFoodResponse(food);
    }
}