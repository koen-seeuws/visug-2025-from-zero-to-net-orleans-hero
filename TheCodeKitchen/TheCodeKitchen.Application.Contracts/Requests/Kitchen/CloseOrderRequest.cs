namespace TheCodeKitchen.Application.Contracts.Requests.Kitchen;

[GenerateSerializer]
public record CloseOrderRequest(long OrderNumber);