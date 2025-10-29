namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.Communication;

public record ReadMessageResponse(int Number, string From, string To, string Content);