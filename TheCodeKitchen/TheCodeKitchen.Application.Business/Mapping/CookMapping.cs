using TheCodeKitchen.Application.Contracts.Response.Cook;
using Timer = TheCodeKitchen.Core.Domain.Timer;

namespace TheCodeKitchen.Application.Business.Mapping;

public sealed class CookMapping : Profile
{
    public CookMapping()
    {
        CreateMap<Cook, CreateCookResponse>();
        CreateMap<Cook, GetCookResponse>();
        CreateMap<Message, ReadMessageResponse>();
        CreateMap<Timer, GetTimerResponse>();
    }
}