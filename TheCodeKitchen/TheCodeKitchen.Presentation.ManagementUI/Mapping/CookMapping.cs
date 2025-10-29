using AutoMapper;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Cook;
using TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Mapping;

public sealed class CookMapping : Profile
{
    public CookMapping()
    {
        CreateMap<GetCookResponse, CookTableRecordModel>();
        CreateMap<CreateCookResponse, CookTableRecordModel>();
        CreateMap<CookJoinedEvent, CookTableRecordModel>();
    }
}