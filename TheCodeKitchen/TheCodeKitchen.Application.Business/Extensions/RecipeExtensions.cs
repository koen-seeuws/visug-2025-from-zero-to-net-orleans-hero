using TheCodeKitchen.Application.Contracts.Response.CookBook;

namespace TheCodeKitchen.Application.Business.Extensions;

public static class RecipeExtensions
{
    public static string GetRecipeComboIdentifier(this IEnumerable<string> ingredientNames)
    {
        const char separator = '-';

        ingredientNames = ingredientNames
            .Select(i => i.Trim().ToCamelCase())
            .Order();

        return string.Join(separator, ingredientNames);
    }

    public static TimeSpan GetMinimumPreparationTime(this GetRecipeResponse recipe, ICollection<GetRecipeResponse> allRecipes)
    {
        var timeToPrepare = recipe.Steps.Sum(s => s.Time);

        var longestTimeToPrepareIngredient = recipe.Ingredients
            .Select(i =>
            {
                var timeToPrepareIngredient = i.Steps.Sum(s => s.Time);
                var ingredientRecipe = allRecipes
                    .FirstOrDefault(r => r.Name.Equals(i.Name, StringComparison.InvariantCultureIgnoreCase));
                if (ingredientRecipe is not null)
                    timeToPrepareIngredient += ingredientRecipe.GetMinimumPreparationTime(allRecipes);
                return timeToPrepareIngredient;
            })
            .DefaultIfEmpty(TimeSpan.Zero)
            .Max();
        
        return timeToPrepare + longestTimeToPrepareIngredient;
    }
}