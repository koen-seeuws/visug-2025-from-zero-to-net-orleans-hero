using TheCodeKitchen.Application.Business.Helpers;
using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed partial class EquipmentGrain
{
    private Task OnNextMomentEvent(NextMomentEvent nextMomentEvent, StreamSequenceToken _)
    {
        if (state.State.MixtureTime.HasValue)
            state.State.MixtureTime += nextMomentEvent.TimePerMoment;

        foreach (var food in state.State.Foods)
        {
            food.Temperature = TemperatureHelper.CalculateNextMomentFoodTemperature(
                nextMomentEvent.TimePerMoment,
                food.Temperature,
                state.State.Temperature ?? nextMomentEvent.Temperature,
                state.State.TemperatureTransferRate ?? RoomTemperatureTransferRate.Value
            );
        }

        return Task.CompletedTask;
    }
}