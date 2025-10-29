namespace TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

[GenerateSerializer]
public record KitchenOrderRatingUpdatedEvent(long Order, double Rating);