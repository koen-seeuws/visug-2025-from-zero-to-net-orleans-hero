namespace TheCodeKitchen.Application.Constants;

public static class MinimumTimeBetweenOrders
{
    public static TimeSpan Minimum { get; } = TimeSpan.FromMinutes(1);
    public static TimeSpan Maximum { get; } = TimeSpan.FromHours(4);
}