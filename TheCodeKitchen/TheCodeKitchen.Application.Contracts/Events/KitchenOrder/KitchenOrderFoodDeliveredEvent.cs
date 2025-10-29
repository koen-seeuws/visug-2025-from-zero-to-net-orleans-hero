namespace TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

[GenerateSerializer]
public record KitchenOrderFoodDeliveredEvent(long Number, string Food, double Rating);