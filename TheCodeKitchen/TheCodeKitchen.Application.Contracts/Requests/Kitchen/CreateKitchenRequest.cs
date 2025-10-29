namespace TheCodeKitchen.Application.Contracts.Requests.Kitchen;

[GenerateSerializer]
public record CreateKitchenRequest(string? Name, Guid GameId);