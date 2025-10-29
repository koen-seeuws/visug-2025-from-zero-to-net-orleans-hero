using TheCodeKitchen.Application.Contracts.Events.Cook;

namespace TheCodeKitchen.Application.Contracts.Realtime;

public interface IRealTimeCookService
{
    Task SendMessageReceivedEvent(MessageReceivedEvent @event);
    Task SendTimerElapsedEvent(string username, TimerElapsedEvent @event);
}