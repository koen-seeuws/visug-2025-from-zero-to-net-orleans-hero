namespace TheCodeKitchen.Application.Contracts.Requests.Cook;

[GenerateSerializer]
public record ReceiveMessageRequest(string From, string To, string Content);