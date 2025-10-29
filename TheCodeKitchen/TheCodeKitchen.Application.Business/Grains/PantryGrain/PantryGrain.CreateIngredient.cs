using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Requests.Pantry;
using TheCodeKitchen.Application.Contracts.Response.Pantry;

namespace TheCodeKitchen.Application.Business.Grains.PantryGrain;

public sealed partial class PantryGrain
{
    public async Task<Result<CreateIngredientResponse>> CreateIngredient(CreateIngredientRequest request)
    {
        var name = request.Name.Trim().ToCamelCase();
        
        if (state.State.Ingredients.Any(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return new AlreadyExistsError($"The ingredient {name} already exists");

        var ingredient = new PantryIngredient(name);
        
        state.State.Ingredients.Add(ingredient);

        await state.WriteStateAsync();

        return mapper.Map<CreateIngredientResponse>(ingredient);
    }
}