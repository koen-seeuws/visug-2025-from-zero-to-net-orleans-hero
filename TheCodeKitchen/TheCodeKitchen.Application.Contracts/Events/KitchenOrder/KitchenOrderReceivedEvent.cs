using TheCodeKitchen.Application.Contracts.Models.Order;

namespace TheCodeKitchen.Application.Contracts.Events.KitchenOrder;

[GenerateSerializer]
public record NewKitchenOrderEvent(long Number, ICollection<FoodRequestDto> RequestedFoods);