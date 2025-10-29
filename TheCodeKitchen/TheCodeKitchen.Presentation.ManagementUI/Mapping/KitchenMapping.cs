using AutoMapper;
using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;
using TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;
using TheCodeKitchen.Presentation.ManagementUI.Models.ViewModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Mapping;

public sealed class KitchenMapping : Profile
{
    public KitchenMapping()
    {
        CreateMap<GetKitchenResponse, KitchenTableRecordModel>();
        CreateMap<CreateKitchenResponse, KitchenTableRecordModel>();
        CreateMap<KitchenCreatedEvent, KitchenTableRecordModel>();

        CreateMap<MessageDeliveredEvent, MessageViewModel>();
    }
}