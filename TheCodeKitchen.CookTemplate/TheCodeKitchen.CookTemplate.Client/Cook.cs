using TheCodeKitchen.CookTemplate.Contracts.Events.Communication;
using TheCodeKitchen.CookTemplate.Contracts.Events.Order;
using TheCodeKitchen.CookTemplate.Contracts.Events.Timer;

namespace TheCodeKitchen.CookTemplate.Client;

public class Cook(TheCodeKitchenClient theCodeKitchenClient)
{
    protected string KitchenCode = "XXXX"; //TODO: Replace with your kitchen code
    protected string Username = "YYYY"; //TODO: Replace with your username
    protected string Password = "ZZZZ"; //TODO: Replace with your password

    protected Action<KitchenOrderCreatedEvent> OnKitchenOrderCreatedEvent = async kitchenOrderCreatedEvent =>
    {
        // TODO: Implement logic here
    };

    protected Action<TimerElapsedEvent> OnTimerElapsedEvent = async timerElapsedEvent =>
    {
        // TODO: Implement logic here
    };

    protected Action<MessageReceivedEvent> OnMessageReceivedEvent = async messageReceivedEvent =>
    {
        // TODO: Implement logic here
    };

    public virtual async Task StartCooking(CancellationToken cancellationToken = default)
    {
        await theCodeKitchenClient.Connect(
            Username,
            Password,
            KitchenCode,
            OnKitchenOrderCreatedEvent,
            OnTimerElapsedEvent,
            OnMessageReceivedEvent,
            cancellationToken
        );
    }
}