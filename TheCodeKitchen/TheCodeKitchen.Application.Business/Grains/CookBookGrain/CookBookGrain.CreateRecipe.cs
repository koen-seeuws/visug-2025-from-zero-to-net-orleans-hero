using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Requests.CookBook;
using TheCodeKitchen.Application.Contracts.Response.CookBook;

namespace TheCodeKitchen.Application.Business.Grains.CookBookGrain;

public sealed partial class CookBookGrain
{
    public async Task<Result<CreateRecipeResponse>> CreateRecipe(CreateRecipeRequest request)
    {
        var newRecipeName = request.Name.Trim().ToCamelCase();

        // Check if recipe already exists
        var availableRecipes = state.State.Recipes.Select(r => r.Name).ToList();

        if (availableRecipes.Any(i => i.Equals(newRecipeName, StringComparison.OrdinalIgnoreCase)))
            return new AlreadyExistsError($"The recipe {newRecipeName} already exists");

        // Check if name is already used for an ingredient in pantry
        var pantryGrain = GrainFactory.GetGrain<IPantryGrain>(state.State.Id);
        var pantryIngredientsResult = await pantryGrain.GetIngredients();

        if (!pantryIngredientsResult.Succeeded)
        {
            return pantryIngredientsResult.Error;
        }

        var availableIngredients = pantryIngredientsResult.Value.Select(i => i.Name).ToList();

        if (availableIngredients.Any(i => i.Equals(newRecipeName, StringComparison.OrdinalIgnoreCase)))
            return new AlreadyExistsError($"{newRecipeName} already exists as an ingredient in the pantry");

        // Check if recipe ingredients combination is unique
        var newIngredientCombo = request.Ingredients
            .Select(i => i.Name)
            .GetRecipeComboIdentifier();

        var existingRecipe = state.State.Recipes
            .FirstOrDefault(recipe =>
                recipe.Ingredients
                    .Select(i => i.Name)
                    .GetRecipeComboIdentifier()
                    .Equals(newIngredientCombo, StringComparison.OrdinalIgnoreCase)
            );

        if (existingRecipe is not null)
            return new AlreadyExistsError(
                $"This combination of ingredients is already used for recipe {existingRecipe.Name}");

        // Start crafting recipe
        var newRecipeIngredients = new List<RecipeIngredient>();

        foreach (var necessaryIngredient in request.Ingredients)
        {
            var necessaryIngredientName = necessaryIngredient.Name.Trim().ToCamelCase();

            var isRecipe = availableRecipes.Contains(necessaryIngredientName);
            var isIngredient = availableIngredients.Contains(necessaryIngredientName);

            switch (isRecipe)
            {
                // Check if necessary ingredient is available in pantry or as a recipe
                case false when !isIngredient:
                    return new InvalidRecipeError(
                        $"The ingredient {necessaryIngredientName} is not available in the pantry or as a recipe");
                case true when necessaryIngredient.Steps.Count != 0:
                    return new InvalidRecipeError("The subrecipes in a new recipe should not contain any steps");
            }


            var necessaryIngredientSteps = necessaryIngredient.Steps
                .Select(i =>
                {
                    var equipmentType = i.EquipmentType.Trim().ToCamelCase();
                    return new RecipeStep(equipmentType, i.Time);
                })
                .ToList();

            var newRecipeIngredient = new RecipeIngredient(necessaryIngredientName, necessaryIngredientSteps);
            newRecipeIngredients.Add(newRecipeIngredient);
        }

        var newRecipeSteps = request.Steps
            .Select(i =>
            {
                var equipmentType = i.EquipmentType.Trim().ToCamelCase();
                return new RecipeStep(equipmentType, i.Time);
            })
            .ToList();

        var newRecipe = new Recipe(newRecipeName, newRecipeIngredients, newRecipeSteps);

        state.State.Recipes.Add(newRecipe);
        await state.WriteStateAsync();

        return mapper.Map<CreateRecipeResponse>(newRecipe);
    }
}