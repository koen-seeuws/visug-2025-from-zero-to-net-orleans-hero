namespace TheCodeKitchen.Application.Contracts.Requests.Kitchen;

[GenerateSerializer]
public record SendMessageRequest(string? To, string Content);