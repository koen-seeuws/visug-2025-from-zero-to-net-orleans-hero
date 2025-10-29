using TheCodeKitchen.Application.Contracts.Events.Cook;

namespace TheCodeKitchen.Infrastructure.AzureSignalR.Services;

public sealed class RealTimeCookService(HubContextProvider hubContextProvider) : IRealTimeCookService
{
    public async Task SendMessageReceivedEvent(MessageReceivedEvent @event)
    {
        var cookHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.CookHub);
        await cookHubContext.Clients.User(@event.To).SendAsync(nameof(MessageReceivedEvent), @event);
    }

    public async Task SendTimerElapsedEvent(string username, TimerElapsedEvent @event)
    {
        var cookHubContext = await hubContextProvider.GetHubContextAsync(HubConstants.CookHub);
        await cookHubContext.Clients.User(username).SendAsync(nameof(TimerElapsedEvent), @event);
    }
}