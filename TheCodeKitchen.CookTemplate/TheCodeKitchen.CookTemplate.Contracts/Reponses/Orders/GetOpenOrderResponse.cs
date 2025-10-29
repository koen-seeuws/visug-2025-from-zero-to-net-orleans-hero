namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.Orders;

public record GetOpenOrderResponse(long Number, ICollection<string> RequestedFoods, ICollection<string> DeliveredFoods);