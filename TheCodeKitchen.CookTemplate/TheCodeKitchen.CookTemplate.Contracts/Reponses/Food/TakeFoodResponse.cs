using TheCodeKitchen.CookTemplate.Contracts.Reponses.CookBook;

namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.Food;

public record TakeFoodResponse(string Name, double Temperature, ICollection<RecipeStepDto> Steps);