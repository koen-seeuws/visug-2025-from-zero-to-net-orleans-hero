using TheCodeKitchen.Application.Contracts.Requests.CookBook;
using TheCodeKitchen.Application.Contracts.Response.CookBook;

namespace TheCodeKitchen.Application.Contracts.Grains;

public interface ICookBookGrain : IGrainWithGuidKey
{
    Task<Result<IEnumerable<GetRecipeResponse>>> GetRecipes();
    Task<Result<CreateRecipeResponse>> CreateRecipe(CreateRecipeRequest request);
}