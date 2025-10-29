namespace TheCodeKitchen.Application.Contracts.Response.Kitchen;

[GenerateSerializer]
public record GetKitchenResponse(
    Guid Id,
    string Name,
    string? Code,
    Guid Game,
    List<string> Cooks,
    List<long> Orders,
    List<long> OpenOrders,
    Dictionary<long, double> OrderRatings
);