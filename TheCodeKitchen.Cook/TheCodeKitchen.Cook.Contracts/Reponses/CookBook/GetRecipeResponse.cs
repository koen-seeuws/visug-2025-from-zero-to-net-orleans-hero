namespace TheCodeKitchen.Cook.Contracts.Reponses.CookBook;

public record GetRecipeResponse(string Name, ICollection<RecipeStepDto> Steps, ICollection<RecipeIngredientDto> Ingredients);