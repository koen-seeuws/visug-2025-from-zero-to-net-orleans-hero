using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Requests.Equipment;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    public async Task<Result<TheCodeKitchenUnit>> AddFood(AddFoodRequest request)
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

        var cookGrain = GrainFactory.GetGrain<ICookGrain>(state.State.Kitchen, request.Cook);
        var releaseFoodResult = await cookGrain.ReleaseFood();

        if (!releaseFoodResult.Succeeded)
            return releaseFoodResult.Error;

        var food = mapper.Map<Food>(releaseFoodResult.Value.Food);

        state.State.MixtureTime ??= TimeSpan.Zero;

        if (state.State.Foods.Count <= 0)
        {
            // Subscribe to NextMomentEvent if first food is added
            var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
            var stream = streamProvider.GetStream<NextMomentEvent>(nameof(NextMomentEvent), state.State.Game);
            streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle =
                await stream.SubscribeAsync(OnNextMomentEvent);
            await streamSubscriptionHandles.WriteStateAsync();
        }

        state.State.Foods.Add(food);
        await state.WriteStateAsync();

        return TheCodeKitchenUnit.Value;
    }
}