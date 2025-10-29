namespace TheCodeKitchen.CookTemplate.Contracts.Events.Order;

public record KitchenOrderCreatedEvent(long Number, ICollection<string> RequestedFoods);