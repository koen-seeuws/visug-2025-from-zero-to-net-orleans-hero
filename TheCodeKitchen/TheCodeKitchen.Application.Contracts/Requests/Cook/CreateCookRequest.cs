namespace TheCodeKitchen.Application.Contracts.Requests.Cook;

[GenerateSerializer]
public record CreateCookRequest(string Username, string PasswordHash, Guid Game, Guid Kitchen);