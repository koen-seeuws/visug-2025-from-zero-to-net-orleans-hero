using TheCodeKitchen.Application.Contracts.Response.CookBook;

namespace TheCodeKitchen.Application.Business.Grains.CookBookGrain;

public sealed partial class CookBookGrain
{
    public Task<Result<IEnumerable<GetRecipeResponse>>> GetRecipes()
    {
        var recipes = mapper
            .Map<List<GetRecipeResponse>>(state.State.Recipes)
            .OrderBy(r => r.Name)
            .ToList();
        return Task.FromResult<Result<IEnumerable<GetRecipeResponse>>>(recipes);
    }
}