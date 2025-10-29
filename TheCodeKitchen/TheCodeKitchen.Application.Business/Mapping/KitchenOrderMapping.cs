using TheCodeKitchen.Application.Contracts.Models.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Response.KitchenOrder;

namespace TheCodeKitchen.Application.Business.Mapping;

public sealed class KitchenOrderMapping : Profile
{
    public KitchenOrderMapping()
    {
        CreateMap<KitchenOrder, CreateKitchenOrderResponse>();
        CreateMap<KitchenOrder, GetKitchenOrderResponse>();
        CreateMap<KitchenOrderFoodRequest, KitchenOrderFoodRequestDto>();
        CreateMap<KitchenOrderFoodDelivery, KitchenOrderFoodDeliveryDto>();
    }
}