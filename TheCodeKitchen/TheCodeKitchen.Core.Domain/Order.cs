namespace TheCodeKitchen.Core.Domain;

public sealed class Order(long number, Guid game, List<OrderFoodRequest> requestedFoods)
{
    public long Number { get; set; } = number;
    public Guid Game { get; set; } = game;
    public List<OrderFoodRequest> RequestedFoods { get; set; } = requestedFoods;
}