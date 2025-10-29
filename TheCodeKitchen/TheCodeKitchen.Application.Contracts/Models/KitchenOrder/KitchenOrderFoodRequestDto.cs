namespace TheCodeKitchen.Application.Contracts.Models.KitchenOrder;

[GenerateSerializer]
public record KitchenOrderFoodRequestDto(
    string Food,
    TimeSpan MinimumTimeToPrepareFood,
    double Rating,
    bool Delivered
);