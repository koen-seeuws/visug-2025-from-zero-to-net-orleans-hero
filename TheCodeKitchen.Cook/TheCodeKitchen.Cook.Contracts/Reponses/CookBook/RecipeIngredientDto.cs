namespace TheCodeKitchen.Cook.Contracts.Reponses.CookBook;

public record RecipeIngredientDto(string Name, ICollection<RecipeStepDto> Steps);