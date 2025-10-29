namespace TheCodeKitchen.Application.Contracts.Requests.Kitchen;

[GenerateSerializer]
public record DeliverMessageRequest(string From, string? To, string Content);