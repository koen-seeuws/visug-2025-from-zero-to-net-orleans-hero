using TheCodeKitchen.Application.Contracts.Requests.Equipment;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Initialize(CreateEquipmentRequest request)
    {
        var kitchen = this.GetPrimaryKey();
        var primaryKeyExtensions = this.GetPrimaryKeyString().Split('+');
        var equipmentType = primaryKeyExtensions[1];
        var number = int.Parse(primaryKeyExtensions[2]);

        if (state.RecordExists)
            return new AlreadyExistsError(
                $"The equipment {equipmentType} {number} has already been initialized in kitchen {kitchen}");

        // Adjusted temperature rates per second for realistic heating/cooling
        var (temperatureTransferRate, temperature) = equipmentType switch
        {
            EquipmentType.Bbq => (0.015, 270), // BBQ – medium-fast heating
            EquipmentType.Fridge => (0.002, 4), // Fridge – slow cooling
            EquipmentType.Freezer => (0.0015, -18), // Freezer – very slow cooling
            EquipmentType.Fryer => (0.02, 180), // Fryer – fast heating
            EquipmentType.HotPlate => (0.01, 40), // HotPlate – gentle warming
            EquipmentType.Oven => (0.008, 225), // Oven – medium heating
            EquipmentType.Stove => (0.012, 225), // Stove – medium-high heating
            _ => ((double?)null, (double?)null) // Fallback (kitchen temperature)
        };

        var equipment = new Equipment(request.Game, kitchen, equipmentType, number, temperature,
            temperatureTransferRate);
        state.State = equipment;
        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}