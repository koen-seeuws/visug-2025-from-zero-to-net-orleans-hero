namespace TheCodeKitchen.Application.Contracts.Models.Order;

[GenerateSerializer]
public record FoodRequestDto(string Food, TimeSpan MinimumTimeToPrepareFood);