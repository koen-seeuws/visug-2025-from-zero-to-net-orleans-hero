using TheCodeKitchen.Application.Contracts.Models.Food;

namespace TheCodeKitchen.Application.Contracts.Models.KitchenOrder;

[GenerateSerializer]
public record KitchenOrderFoodDeliveryDto(FoodDto Food, double Rating);