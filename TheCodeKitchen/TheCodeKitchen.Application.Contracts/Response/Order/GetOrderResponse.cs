using TheCodeKitchen.Application.Contracts.Models.Order;

namespace TheCodeKitchen.Application.Contracts.Response.Order;

[GenerateSerializer]
public record GetOrderResponse(long Number, ICollection<FoodRequestDto> RequestedFoods);