using System.Collections.Concurrent;
using System.Text.Json;
using TheCodeKitchen.Cook.Client.Custom;
using TheCodeKitchen.Cook.Contracts.Constants;
using TheCodeKitchen.Cook.Contracts.Requests.Communication;

namespace TheCodeKitchen.Cook.Client.Cooks;

public class EquipmentCoordinator : Cook
{
    private readonly TheCodeKitchenClient _client;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, bool>> _lockedEquipment = new();

    private readonly Dictionary<string, int> _equipments = new(StringComparer.OrdinalIgnoreCase)
    {
        { EquipmentType.Bbq, 8 },
        { EquipmentType.Blender, 4 },
        { EquipmentType.Counter, 30 },
        { EquipmentType.CuttingBoard, 10 },
        { EquipmentType.Freezer, 20 },
        { EquipmentType.Fridge, 20 },
        { EquipmentType.Fryer, 6 },
        { EquipmentType.HotPlate, 15 },
        { EquipmentType.Mixer, 6 },
        { EquipmentType.Oven, 6 },
        { EquipmentType.Stove, 8 },
    };

    public EquipmentCoordinator(TheCodeKitchenClient client, string kitchenCode, string username, string password)
        : base(client)
    {
        _client = client;
        
        KitchenCode = kitchenCode;
        Username = username;
        Password = password;

        OnMessageReceivedEvent = async message =>
        {
            var messageContent = JsonSerializer.Deserialize<MessageContent>(message.Content)!;

            switch (messageContent.Code)
            {
                case MessageCodes.RequestEquipment:
                    await HandleRequest(message.From, messageContent.EquipmentType!);
                    break;

                case MessageCodes.ReleaseEquipment:
                    await HandleRelease(message.From, messageContent.EquipmentType!, messageContent.EquipmentNumber!.Value);
                    break;
            }
            
            var messageConfirmation = new ConfirmMessageRequest(message.Number);
            await _client.ConfirmMessage(messageConfirmation);
        };
    }

    private async Task HandleRequest(string requester, string equipmentType)
    {
        Console.WriteLine($"{Username} - Handling equipment request for user {requester} for type {equipmentType}");
        
        var equipmentNumber = -1;

        // Get total available equipment
        var total = _equipments.GetValueOrDefault(equipmentType, 0);

        if (total > 0)
        {
            // Get or create the thread-safe set of locked equipment
            var locked = _lockedEquipment.GetOrAdd(equipmentType, _ => new ConcurrentDictionary<int, bool>());

            // Find the first available equipment
            for (var i = 0; i < total; i++)
            {
                if (!locked.TryAdd(i, true)) continue;

                equipmentNumber = i; // Successfully locked this equipment
                break;
            }
        }

        // Send grant message
        var grant = new MessageContent(
            MessageCodes.GrantEquipment,
            null, null,
            equipmentType,
            equipmentNumber
        );

        Console.WriteLine($"{Username} - Granting equipment {equipmentType} {equipmentNumber} to user {requester}");
        
        await _client.SendMessage(new SendMessageRequest(requester, JsonSerializer.Serialize(grant)));
    }
    
    private async Task HandleRelease(string releaser, string equipmentType, int equipmentNumber)
    {
        Console.WriteLine($"{Username} - Handling equipment release {equipmentType} {equipmentNumber} from {releaser}");
        if (_lockedEquipment.TryGetValue(equipmentType, out var locked))
            locked.TryRemove(equipmentNumber, out _);
    }
}