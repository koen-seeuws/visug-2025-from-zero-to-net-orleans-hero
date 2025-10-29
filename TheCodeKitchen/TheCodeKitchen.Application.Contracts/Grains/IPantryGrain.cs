using TheCodeKitchen.Application.Contracts.Requests.Pantry;
using TheCodeKitchen.Application.Contracts.Response.Food;
using TheCodeKitchen.Application.Contracts.Response.Pantry;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface IPantryGrain : IGrainWithGuidKey
{
    Task<Result<IEnumerable<GetIngredientResponse>>> GetIngredients();
    Task<Result<CreateIngredientResponse>> CreateIngredient(CreateIngredientRequest request);
    Task<Result<TakeFoodResponse>> TakeFood(TakeFoodFromPantryRequest request);
}