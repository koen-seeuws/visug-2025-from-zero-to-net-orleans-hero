namespace TheCodeKitchen.Application.Contracts.Models.Recipe;

[GenerateSerializer]
public record RecipeIngredientDto(string Name, ICollection<RecipeStepDto> Steps);