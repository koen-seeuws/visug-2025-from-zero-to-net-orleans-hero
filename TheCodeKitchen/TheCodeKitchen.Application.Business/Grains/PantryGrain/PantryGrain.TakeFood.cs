using TheCodeKitchen.Application.Contracts.Models.Food;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Pantry;
using TheCodeKitchen.Application.Contracts.Response.Food;

namespace TheCodeKitchen.Application.Business.Grains.PantryGrain;

public sealed partial class PantryGrain
{
    public async Task<Result<TakeFoodResponse>> TakeFood(TakeFoodFromPantryRequest request)
    {
        var ingredient = state.State.Ingredients.FirstOrDefault(i =>
            string.Equals(i.Name, request.Ingredient, StringComparison.InvariantCultureIgnoreCase));

        if (ingredient is null)
            return new NotFoundError($"The ingredient with name {request.Ingredient} was not found in the pantry");

        var cookGrain = GrainFactory.GetGrain<ICookGrain>(request.Kitchen, request.Cook);
        
        var getKitchenResult = await cookGrain.GetKitchen();

        if (!getKitchenResult.Succeeded)
            return getKitchenResult.Error;
        
        var food = new Food(ingredient.Name, state.State.Temperature, getKitchenResult.Value.Game, getKitchenResult.Value.Id);
        var foodDto = mapper.Map<FoodDto>(food);
        
        var holdFoodRequest = new HoldFoodRequest(foodDto);
        var holdFoodResult = await cookGrain.HoldFood(holdFoodRequest);
        
        if(!holdFoodResult.Succeeded)
            return holdFoodResult.Error;
        
        return mapper.Map<TakeFoodResponse>(food);
    }
}