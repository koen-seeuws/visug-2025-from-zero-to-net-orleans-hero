namespace TheCodeKitchen.Application.Contracts.Response.Order;

[GenerateSerializer]
public record GetOpenOrderResponse(long Number, ICollection<string> RequestedFoods, ICollection<string> DeliveredFoods);