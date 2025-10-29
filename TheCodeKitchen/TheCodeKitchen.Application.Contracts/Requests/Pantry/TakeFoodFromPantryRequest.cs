namespace TheCodeKitchen.Application.Contracts.Requests.Pantry;

[GenerateSerializer]
public record TakeFoodFromPantryRequest(string Ingredient, Guid Kitchen, string Cook);