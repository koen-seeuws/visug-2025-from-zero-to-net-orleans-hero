using TheCodeKitchen.Application.Contracts.Response.Order;

namespace TheCodeKitchen.Application.Business.Mapping;

public sealed class OrderMapping : Profile
{
    public OrderMapping()
    {
        CreateMap<Order, GetOrderResponse>();
    }
}