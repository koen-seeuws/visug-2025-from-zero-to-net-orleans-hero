using System.Text.Json;
using System.Threading.Channels;
using TheCodeKitchen.Cook.Client.Custom;
using TheCodeKitchen.Cook.Contracts.Events.Order;
using TheCodeKitchen.Cook.Contracts.Requests.Communication;

namespace TheCodeKitchen.Cook.Client.Cooks;

public class HeadChef : Cook
{
    private readonly TheCodeKitchenClient _theCodeKitchenClient;
    private readonly string _equipmentCoordinator;
    private readonly string[] _chefs;
    private int _chefLoadCounter;
    private readonly SemaphoreSlim _holdFoodSemaphore = new(1, 1);

    public HeadChef(TheCodeKitchenClient theCodeKitchenClient, string kitchenCode,
        string username, string password, string equipmentCoordinator, string[] chefs) : base(theCodeKitchenClient)
    {
        _theCodeKitchenClient = theCodeKitchenClient;

        Username = username;
        Password = password;
        KitchenCode = kitchenCode;

        _equipmentCoordinator = equipmentCoordinator;
        _chefs = chefs;

        OnKitchenOrderCreatedEvent = async kitchenOrderCreatedEvent => await SendCookCommand(kitchenOrderCreatedEvent);

        OnMessageReceivedEvent = async messageReceivedEvent =>
        {
            var message = new Message(
                messageReceivedEvent.Number,
                messageReceivedEvent.From,
                messageReceivedEvent.To,
                JsonSerializer.Deserialize<MessageContent>(messageReceivedEvent.Content)!
            );
            await ProcessMessage(message);
        };
    }

    private async Task SendCookCommand(KitchenOrderCreatedEvent kitchenOrderCreatedEvent)
    {
        Console.WriteLine($"{Username} - New Order Received - {JsonSerializer.Serialize(kitchenOrderCreatedEvent)}");

        var cookFoodTasks = kitchenOrderCreatedEvent.RequestedFoods.Select(async (food, index) =>
        {
            // Spreading cooking load evenly among chefs
            var to = _chefs[_chefLoadCounter++ % _chefs.Length];

            // Sending the cook the order to cook the food
            var messageContent = new MessageContent(
                MessageCodes.CookFood,
                kitchenOrderCreatedEvent.Number,
                food,
                null,
                null
            );
            var sendMessage = new SendMessageRequest(to, JsonSerializer.Serialize(messageContent));
            await _theCodeKitchenClient.SendMessage(sendMessage);
        });

        await Task.WhenAll(cookFoodTasks);
    }

    private async Task ProcessMessage(Message message)
    {
        switch (message.Content.Code)
        {
            case MessageCodes.FoodReady:
            {
                Console.WriteLine(
                    $"{Username} - Received Food ready - Order: {message.Content.Order}, {message.Content.Food} - Equipment {message.Content.EquipmentType} {message.Content.EquipmentNumber}");

                var orders = await _theCodeKitchenClient.ViewOpenOrders();
                var order = orders.FirstOrDefault(o => o.Number == message.Content.Order!.Value);

                await _holdFoodSemaphore.WaitAsync();
                try
                {
                    await _theCodeKitchenClient.TakeFoodFromEquipment(message.Content.EquipmentType!,
                        message.Content.EquipmentNumber!.Value);
                    if (order != null)
                        await _theCodeKitchenClient.DeliverFoodToOrder(message.Content.Order!.Value);
                    else
                        await _theCodeKitchenClient.ThrowFoodAway(); // Order completed already

                    await ReleaseEquipment(message.Content.EquipmentType!, message.Content.EquipmentNumber!.Value);
                }
                finally
                {
                    _holdFoodSemaphore.Release();
                }
                
                if(order == null)
                    break; // Order completed already

                var deliveredGroups = order.DeliveredFoods
                    .Append(message.Content.Food)
                    .GroupBy(f => f!.Trim().ToLowerInvariant())
                    .ToDictionary(g => g.Key, g => g.Count());

                var allDelivered = order.RequestedFoods
                    .GroupBy(f => f.Trim().ToLowerInvariant())
                    .All(req => deliveredGroups.TryGetValue(req.Key, out var deliveredCount) &&
                                deliveredCount >= req.Count());

                if (allDelivered)
                {
                    Console.WriteLine($"{Username} - Completing order {message.Content.Order!.Value}");
                    await _theCodeKitchenClient.CompleteOrder(message.Content.Order!.Value);
                }

                break;
            }
        }

        var confirmMessageRequest = new ConfirmMessageRequest(message.Number);
        await _theCodeKitchenClient.ConfirmMessage(confirmMessageRequest);
    }

    private async Task ReleaseEquipment(string equipmentType, int number)
    {
        var release = new MessageContent(MessageCodes.ReleaseEquipment, null, null, equipmentType, number);
        await _theCodeKitchenClient.SendMessage(new SendMessageRequest(_equipmentCoordinator,
            JsonSerializer.Serialize(release)));
    }
}