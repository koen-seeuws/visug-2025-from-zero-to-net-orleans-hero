using TheCodeKitchen.Application.Business.Extensions;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    private async Task<Result<TheCodeKitchenUnit>> MixFood()
    {
        if (!state.RecordExists)
        {
            var kitchen = this.GetPrimaryKey();
            var primaryKeyExtensions = this.GetPrimaryKeyString().Split('+');
            var equipmentType = primaryKeyExtensions[1];
            var number = int.Parse(primaryKeyExtensions[2]);

            return new NotFoundError(
                $"The equipment {equipmentType} {number} does not exist in kitchen {kitchen}");
        }

        var recipes = GrainFactory.GetGrain<ICookBookGrain>(Guid.Empty);
        var recipesResult = await recipes.GetRecipes();

        if (!recipesResult.Succeeded)
            return recipesResult.Error;

        var necessaryIngredientCombo = state.State.Foods
            .Select(i => i.Name)
            .GetRecipeComboIdentifier();

        var recipe = recipesResult.Value
            .FirstOrDefault(r => r.Ingredients
                .Select(i => i.Name)
                .GetRecipeComboIdentifier()
                .Equals(necessaryIngredientCombo, StringComparison.OrdinalIgnoreCase)
            );

        var food = new Food(
            recipe?.Name ?? UnknownMixture.Value,
            state.State.Foods.Select(f => f.Temperature).Average(),
            state.State.Game,
            state.State.Kitchen,
            state.State.Foods
        );

        state.State.Foods = [food];
        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}