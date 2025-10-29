using Microsoft.AspNetCore.SignalR;

namespace TheCodeKitchen.Presentation.ManagementUI.Hubs;

public sealed class GameManagementHub : Hub
{
    // This Hub is actually being used for sending out events
    // Empty body is necessary for Azure SignalR to recognize the Hub
}