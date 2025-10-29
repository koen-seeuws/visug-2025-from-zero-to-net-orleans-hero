namespace TheCodeKitchen.Application.Contracts.Events.Kitchen;

[GenerateSerializer]
public record CookJoinedEvent(string Username, Guid Kitchen);