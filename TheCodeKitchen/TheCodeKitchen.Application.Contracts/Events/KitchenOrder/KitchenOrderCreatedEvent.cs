namespace TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

public record KitchenOrderCreatedEvent(long Number, ICollection<string> RequestedFoods);