namespace TheCodeKitchen.Cook.Contracts.Events.Communication;

public record MessageReceivedEvent(int Number, string From, string To, string Content);