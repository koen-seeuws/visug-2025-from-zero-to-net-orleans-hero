using TheCodeKitchen.Application.Contracts.Models.Order;

namespace TheCodeKitchen.Application.Contracts.Events.Game;

[GenerateSerializer]
public record OrderGeneratedEvent(long Number, ICollection<FoodRequestDto> RequestedFoods);