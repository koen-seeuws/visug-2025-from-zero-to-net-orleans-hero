using AutoMapper;
using TheCodeKitchen.Application.Contracts.Events.GameManagement;
using TheCodeKitchen.Application.Contracts.Response.Game;
using TheCodeKitchen.Presentation.ManagementUI.Models.TableRecordModels;

namespace TheCodeKitchen.Presentation.ManagementUI.Mapping;

public sealed class GameMapping : Profile
{
    public GameMapping()
    {
        CreateMap<GetGameResponse, GameTableRecordModel>();
        CreateMap<CreateGameResponse, GameTableRecordModel>();
        CreateMap<GameCreatedEvent, GameTableRecordModel>();
    }
}