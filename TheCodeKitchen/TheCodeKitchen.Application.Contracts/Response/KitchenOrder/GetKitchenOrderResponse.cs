using TheCodeKitchen.Application.Contracts.Models.KitchenOrder;

namespace TheCodeKitchen.Application.Contracts.Response.KitchenOrder;

[GenerateSerializer]
public record GetKitchenOrderResponse(
    long Number,
    TimeSpan Time,
    bool Completed,
    double CompletenessRating,
    Guid Game,
    Guid Kitchen,
    ICollection<KitchenOrderFoodRequestDto> RequestedFoods,
    ICollection<KitchenOrderFoodDeliveryDto> DeliveredFoods
);