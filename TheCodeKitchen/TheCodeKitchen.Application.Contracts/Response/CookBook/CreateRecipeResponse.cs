using TheCodeKitchen.Application.Contracts.Models.Recipe;

namespace TheCodeKitchen.Application.Contracts.Response.CookBook;

[GenerateSerializer]
public record CreateRecipeResponse(string Name, IEnumerable<RecipeStepDto> Steps, IEnumerable<RecipeIngredientDto> Ingredients);