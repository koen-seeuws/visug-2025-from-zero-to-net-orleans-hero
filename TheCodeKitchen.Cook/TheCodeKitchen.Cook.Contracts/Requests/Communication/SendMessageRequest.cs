namespace TheCodeKitchen.Cook.Contracts.Requests.Communication;

public record SendMessageRequest(string? To, string Content);