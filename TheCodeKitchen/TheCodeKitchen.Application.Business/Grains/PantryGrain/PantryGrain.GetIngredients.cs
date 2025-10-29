using TheCodeKitchen.Application.Contracts.Response.Pantry;

namespace TheCodeKitchen.Application.Business.Grains.PantryGrain;

public sealed partial class PantryGrain
{
    public Task<Result<IEnumerable<GetIngredientResponse>>> GetIngredients()
    {
        var ingredients = mapper
            .Map<List<GetIngredientResponse>>(state.State.Ingredients)
            .OrderBy(i => i.Name)
            .ToList();
        return Task.FromResult<Result<IEnumerable<GetIngredientResponse>>>(ingredients);
    }
}