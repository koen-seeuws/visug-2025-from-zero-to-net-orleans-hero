namespace TheCodeKitchen.Application.Contracts.Events.GameManagement;

[GenerateSerializer]
public record GameCreatedEvent(
    Guid Id,
    string Name,
    double SpeedModifier,
    double Temperature,
    DateTimeOffset? Started,
    bool Paused
);