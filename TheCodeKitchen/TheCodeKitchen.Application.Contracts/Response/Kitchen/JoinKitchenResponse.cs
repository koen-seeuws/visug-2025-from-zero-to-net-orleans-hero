namespace TheCodeKitchen.Application.Contracts.Response.Kitchen;

[GenerateSerializer]
public record JoinKitchenResponse(
    Guid GameId,
    Guid KitchenId,
    string Username,
    string PasswordHash,
    bool isNewCook
);