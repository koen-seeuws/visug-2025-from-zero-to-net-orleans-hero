namespace TheCodeKitchen.Application.Contracts.Events.Cook;

[GenerateSerializer]
public record MessageReceivedEvent(int Number, string From, string To, string Content);