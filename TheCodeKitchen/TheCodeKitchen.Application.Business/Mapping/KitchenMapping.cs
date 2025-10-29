using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Mapping;

public sealed class KitchenMapping : Profile
{
    public KitchenMapping()
    {
        CreateMap<Kitchen, CreateKitchenResponse>();
        CreateMap<Kitchen, GetKitchenResponse>();
    }
}