namespace TheCodeKitchen.Cook.Contracts.Reponses.Communication;

public record ReadMessageResponse(int Number, string From, string To, string Content);