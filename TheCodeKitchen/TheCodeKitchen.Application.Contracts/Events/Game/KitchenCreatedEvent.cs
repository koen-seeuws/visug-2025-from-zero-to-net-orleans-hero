namespace TheCodeKitchen.Application.Contracts.Events.Game;

[GenerateSerializer]
public record KitchenCreatedEvent(
    Guid Id,
    string Name,
    string Code
);