namespace TheCodeKitchen.Application.Constants;

public static class MaximumTimeBetweenOrders
{
    public static TimeSpan Minimum { get; } = TimeSpan.FromMinutes(10);
    public static TimeSpan Maximum { get; } = TimeSpan.FromHours(12);
}