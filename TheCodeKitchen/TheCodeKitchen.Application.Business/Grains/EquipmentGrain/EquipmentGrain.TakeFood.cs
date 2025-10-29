using TheCodeKitchen.Application.Contracts.Models.Food;
using TheCodeKitchen.Application.Contracts.Requests.Cook;
using TheCodeKitchen.Application.Contracts.Requests.Equipment;
using TheCodeKitchen.Application.Contracts.Response.Food;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    public async Task<Result<TakeFoodResponse>> TakeFood(TakeFoodFromEquipmentRequest request)
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

        if (state.State.Foods.Count <= 0)
            return new EquipmentEmptyError(
                $"The equipment {state.State.EquipmentType} {state.State.Number} does not contain any food");

        if (state.State.Foods.Count > 1)
        {
            var mixResult = await MixFood();
            if (!mixResult.Succeeded)
                return mixResult.Error;
        }

        var food = state.State.Foods.First();
        
        if (state.State.MixtureTime.HasValue)
        {
            var isSteppable = EquipmentType.Steppable
                .Any(et => et.Equals(state.State.EquipmentType, StringComparison.OrdinalIgnoreCase)
                );

            if (isSteppable)
            {
                var lastStep = food.Steps.LastOrDefault();
                if (lastStep is not null &&
                    lastStep.EquipmentType.Equals(state.State.EquipmentType, StringComparison.OrdinalIgnoreCase))
                {
                    lastStep.Time += state.State.MixtureTime.Value;
                }
                else
                {
                    var step = new RecipeStep(state.State.EquipmentType, state.State.MixtureTime.Value);
                    food.Steps.Add(step);
                }
            }
        }

        var foodDto = mapper.Map<FoodDto>(food);
        var cookGrain = GrainFactory.GetGrain<ICookGrain>(state.State.Kitchen, request.Cook);
        var holdFoodRequest = new HoldFoodRequest(foodDto);
        var holdFoodResult = await cookGrain.HoldFood(holdFoodRequest);

        if (!holdFoodResult.Succeeded)
            return holdFoodResult.Error;

        state.State.MixtureTime = null;
        state.State.Foods.Clear();
        await state.WriteStateAsync();

        if (streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle is not null)
        {
            // Unsubscribe from NextMomentEvent if last food is taken
            await streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle.UnsubscribeAsync();
            streamSubscriptionHandles.State.NextMomentStreamSubscriptionHandle = null;
            await streamSubscriptionHandles.WriteStateAsync();
        }

        return mapper.Map<TakeFoodResponse>(food);
    }
}