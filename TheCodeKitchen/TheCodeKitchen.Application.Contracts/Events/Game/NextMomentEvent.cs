namespace TheCodeKitchen.Application.Contracts.Events.Game;

[GenerateSerializer]
public record NextMomentEvent(double Temperature, TimeSpan TimePerMoment, TimeSpan TimePassed, TimeSpan? NextMomentDelay);