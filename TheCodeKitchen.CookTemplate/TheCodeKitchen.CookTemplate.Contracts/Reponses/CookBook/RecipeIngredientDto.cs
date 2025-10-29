namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.CookBook;

public record RecipeIngredientDto(string Name, ICollection<RecipeStepDto> Steps);