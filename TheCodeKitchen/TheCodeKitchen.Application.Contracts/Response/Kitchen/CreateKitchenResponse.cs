namespace TheCodeKitchen.Application.Contracts.Response.Kitchen;

[GenerateSerializer]
public record CreateKitchenResponse(
    Guid Id,
    string Name,
    string Code,
    Guid Game
);