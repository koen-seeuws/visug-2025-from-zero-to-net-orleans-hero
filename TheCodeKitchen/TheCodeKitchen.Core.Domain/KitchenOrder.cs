namespace TheCodeKitchen.Core.Domain;

public sealed class KitchenOrder(long number, List<KitchenOrderFoodRequest> requestedFoods, Guid game, Guid kitchen)
{
    public long Number { get; set; } = number;
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
    public bool Completed { get; set; }
    public double CompletenessRating { get; set; } = 0.0;
    public double TotalRating { get; set; } = 0.0;

    public Guid Game { get; set; } = game;
    public Guid Kitchen { get; set; } = kitchen;

    public List<KitchenOrderFoodRequest> RequestedFoods { get; set; } = requestedFoods;

    public List<KitchenOrderFoodDelivery> DeliveredFoods { get; set; } = [];
}