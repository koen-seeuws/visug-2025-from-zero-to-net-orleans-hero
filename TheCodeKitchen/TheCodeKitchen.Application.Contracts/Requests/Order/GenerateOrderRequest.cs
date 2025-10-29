namespace TheCodeKitchen.Application.Contracts.Requests.Order;

[GenerateSerializer]
public record GenerateOrderRequest(short MinimumItemsPerOrder, short MaximumItemsPerOrder);