using TheCodeKitchen.Application.Contracts.Models.Recipe;

namespace TheCodeKitchen.Application.Contracts.Response.CookBook;

[GenerateSerializer]
public record GetRecipeResponse(string Name, ICollection<RecipeStepDto> Steps, ICollection<RecipeIngredientDto> Ingredients);