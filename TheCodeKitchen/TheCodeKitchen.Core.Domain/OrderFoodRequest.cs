namespace TheCodeKitchen.Core.Domain;

public sealed class OrderFoodRequest(string food, TimeSpan minimumTimeToPrepareFood)
{
    public string Food { get; set; } = food;
    public TimeSpan MinimumTimeToPrepareFood { get; set; } = minimumTimeToPrepareFood;
}