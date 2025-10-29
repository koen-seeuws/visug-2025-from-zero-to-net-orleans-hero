using TheCodeKitchen.Cook.Contracts.Reponses.CookBook;

namespace TheCodeKitchen.Cook.Contracts.Reponses.Food;

public record TakeFoodResponse(string Name, double Temperature, ICollection<RecipeStepDto> Steps);