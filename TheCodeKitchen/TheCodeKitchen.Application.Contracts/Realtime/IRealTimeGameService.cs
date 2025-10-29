using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Events.Kitchen;

namespace TheCodeKitchen.Application.Contracts.Realtime;

public interface IRealTimeGameService
{
    Task SendKitchenCreatedEvent(Guid gameId, KitchenCreatedEvent @event);
    Task SendCookJoinedEvent(Guid gameId, CookJoinedEvent @event);
    Task SendGamePausedOrResumedEvent(Guid gameId, GamePausedOrResumedEvent @event);
    Task SendMomentPassedEvent(Guid gameId, MomentPassedEvent @event);
    Task SendGameResetEvent(Guid gameId, GameResetEvent @event);
}