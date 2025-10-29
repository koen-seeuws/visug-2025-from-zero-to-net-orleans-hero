namespace TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;

public record GameTableRecordModel(
    Guid Id,
    string Name,
    TimeSpan TimePerMoment,
    double SpeedModifier,
    TimeSpan MinimumTimeBetweenOrders,
    double Temperature,
    DateTimeOffset? Started,
    TimeSpan TimePassed,
    bool Paused
);