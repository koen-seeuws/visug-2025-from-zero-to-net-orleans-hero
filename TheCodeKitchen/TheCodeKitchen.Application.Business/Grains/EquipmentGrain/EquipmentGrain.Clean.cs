namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Clean()
    {
        if (!state.RecordExists)
        {
            var kitchen = this.GetPrimaryKey();
            var primaryKeyExtensions = this.GetPrimaryKeyString().Split('+');
            var equipmentType = primaryKeyExtensions[1];
            var number = int.Parse(primaryKeyExtensions[2]);

            return new NotFoundError(
                $"The equipment {equipmentType} {number} does not exist in kitchen {kitchen}");
        }
        
        state.State.MixtureTime = null;
        state.State.Foods.Clear();
        await state.WriteStateAsync();

        if (streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle is null)
            return TheCodeKitchenUnit.Value;
        
        // Unsubscribe from NextMomentEvent if equipment is cleaned
        await streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle.UnsubscribeAsync();
        streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle = null;
        await streamSubscriptionHandles.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}