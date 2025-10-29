using TheCodeKitchen.Application.Contracts.Models.Recipe;

namespace TheCodeKitchen.Application.Contracts.Response.Food;

[GenerateSerializer]
public record TakeFoodResponse(string Name, double Temperature, ICollection<RecipeStepDto> Steps);