using TheCodeKitchen.Application.Contracts.Models.Food;

namespace TheCodeKitchen.Application.Contracts.Requests.Cook;

[GenerateSerializer]
public record  HoldFoodRequest(FoodDto Food);