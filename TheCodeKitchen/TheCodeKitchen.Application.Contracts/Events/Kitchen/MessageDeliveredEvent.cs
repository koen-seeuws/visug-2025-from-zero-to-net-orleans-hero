namespace TheCodeKitchen.Application.Contracts.Events.Kitchen;

[GenerateSerializer]
public record MessageDeliveredEvent(string From, string? To, string Content);