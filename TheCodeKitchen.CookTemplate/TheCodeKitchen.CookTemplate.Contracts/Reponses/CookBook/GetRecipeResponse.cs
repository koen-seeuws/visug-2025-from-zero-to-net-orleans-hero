namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.CookBook;

public record GetRecipeResponse(string Name, ICollection<RecipeStepDto> Steps, ICollection<RecipeIngredientDto> Ingredients);