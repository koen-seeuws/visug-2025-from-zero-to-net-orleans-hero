namespace TheCodeKitchen.Application.Contracts.Requests.Kitchen;

[GenerateSerializer]
public record JoinKitchenRequest(
    string Username,
    string PasswordHash,
    string KitchenCode
);