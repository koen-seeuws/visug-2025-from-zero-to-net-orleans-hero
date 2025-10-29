namespace TheCodeKitchen.Application.Contracts.Events.Cook;

[GenerateSerializer]
public record TimerElapsedEvent(int Number, string? Note);