namespace TheCodeKitchen.Application.Contracts.Response.Order;

[GenerateSerializer]
public record GenerateOrderResponse(TimeSpan MinimumTimeToPrepare);