namespace TheCodeKitchen.Application.Contracts.Response.Cook;

[GenerateSerializer]
public record ReadMessageResponse(int Number, string From, string To, string Content);