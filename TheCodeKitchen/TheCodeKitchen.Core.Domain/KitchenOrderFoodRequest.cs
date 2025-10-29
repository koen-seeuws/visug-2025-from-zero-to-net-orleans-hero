namespace TheCodeKitchen.Core.Domain;

public sealed class KitchenOrderFoodRequest(string food, TimeSpan minimumTimeToPrepareFood)
{
    public string Food { get; set; } = food;
    public TimeSpan MinimumTimeToPrepareFood { get; set; } = minimumTimeToPrepareFood;
    public double Rating { get; set; } = 1.0;
    public bool Delivered { get; set; }
}