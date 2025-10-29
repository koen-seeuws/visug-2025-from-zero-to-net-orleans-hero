using TheCodeKitchen.Application.Contracts.Models.Food;

namespace TheCodeKitchen.Application.Contracts.Response.Cook;

[GenerateSerializer]
public record ReleaseFoodResponse(FoodDto Food);