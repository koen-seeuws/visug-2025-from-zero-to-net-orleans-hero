namespace TheCodeKitchen.Application.Contracts.Requests.Game;

[GenerateSerializer]
public record UpdateGameRequest(
    TimeSpan TimePerMoment,
    double SpeedModifier,
    TimeSpan MinimumTimeBetweenOrders,
    TimeSpan MaximumTimeBetweenOrders,
    short MinimumItemsPerOrder,
    short MaximumItemsPerOrder,
    double OrderSpeedModifier,
    double Temperature
);