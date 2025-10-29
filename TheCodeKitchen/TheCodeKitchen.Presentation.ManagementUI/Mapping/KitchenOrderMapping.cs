using AutoMapper;
using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Response.Order;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Mapping;

public sealed class KitchenOrderMapping : Profile
{
    public KitchenOrderMapping()
    {
        CreateMap<GetOpenOrderResponse, KitchenOrderViewModel>();
        
        CreateMap<KitchenOrderCreatedEvent, KitchenOrderViewModel>()
            .ForMember(dest => dest.DeliveredFoods, opt => opt.MapFrom(src => new List<string>()));
    }
}