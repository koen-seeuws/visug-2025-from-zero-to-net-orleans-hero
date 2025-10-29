using TheCodeKitchen.Application.Contracts.Requests.Kitchen;
using TheCodeKitchen.Application.Contracts.Response.Kitchen;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<CreateKitchenResponse>> Initialize(CreateKitchenRequest request, int count)
    {
        if (state.RecordExists)
            return new AlreadyExistsError($"The kitchen with id {this.GetPrimaryKey()} has already been initialized");

        var id = this.GetPrimaryKey();

        var name = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name))
            name = $"Kitchen {count}";

        var kitchenCodeIndexGrain = GrainFactory.GetGrain<IKitchenManagementGrain>(Guid.Empty);
        var codeResult = await kitchenCodeIndexGrain.GenerateUniqueCode(id);

        if (!codeResult.Succeeded)
            return codeResult.Error;
        
        // Equipment 
        var equipments = new Dictionary<string, int>
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

        // State
        var kitchen = new Kitchen(id, name, codeResult.Value, request.GameId, equipments);
        state.State = kitchen;
        await state.WriteStateAsync();
        
        // Streams
        await SubscribeToNextMomentEvent();
        await SubscribeToOrderGeneratedEvent();
        await SubscribeToKitchenOrderRatingUpdatedEvent();

        return mapper.Map<CreateKitchenResponse>(kitchen);
    }
}