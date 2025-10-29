using TheCodeKitchen.Application.Business.Extensions;
using TheCodeKitchen.Application.Contracts.Requests.Equipment;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed partial class KitchenGrain
{
    public async Task<Result<TheCodeKitchenUnit>> InitializeEquipment()
    {
        var initializeEquipmentTasks = state.State.Equipment
            .SelectMany(e =>
            {
                var initializeEquipmentTypeTasks = Enumerable
                    .Range(0, e.Value)
                    .Select(number =>
                    {
                        var equipmentGrainIdExtension = EquipmentGrainId.Create(e.Key, number);
                        var equipmentGrain =
                            GrainFactory.GetGrain<IEquipmentGrain>(state.State.Id, equipmentGrainIdExtension);
                        var createEquipmentRequest =
                            new CreateEquipmentRequest(state.State.Game, state.State.Id, number);
                        return equipmentGrain.Initialize(createEquipmentRequest);
                    });

                return initializeEquipmentTypeTasks;
            });

        var initializeEquipmentResults = await Task.WhenAll(initializeEquipmentTasks);
        var initializeEquipmentsResult = initializeEquipmentResults.Combine();

        if (!initializeEquipmentsResult.Succeeded)
            return initializeEquipmentsResult.Error;

        return TheCodeKitchenUnit.Value;
    }
}