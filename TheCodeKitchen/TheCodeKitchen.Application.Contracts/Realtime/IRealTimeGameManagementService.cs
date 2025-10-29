using TheCodeKitchen.Application.Contracts.Events.GameManagement;

namespace TheCodeKitchen.Application.Contracts.Realtime;

public interface IRealTimeGameManagementService
{
    Task SendGameCreatedEvent(GameCreatedEvent @event);
}