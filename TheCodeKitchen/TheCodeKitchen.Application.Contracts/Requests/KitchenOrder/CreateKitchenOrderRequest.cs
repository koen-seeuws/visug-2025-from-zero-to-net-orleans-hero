using TheCodeKitchen.Application.Contracts.Models.Order;

namespace TheCodeKitchen.Application.Contracts.Requests.KitchenOrder;

[GenerateSerializer]
public record CreateKitchenOrderRequest(Guid Game, Guid KitchenId, long OrderNumber, ICollection<FoodRequestDto> RequestedFoods);