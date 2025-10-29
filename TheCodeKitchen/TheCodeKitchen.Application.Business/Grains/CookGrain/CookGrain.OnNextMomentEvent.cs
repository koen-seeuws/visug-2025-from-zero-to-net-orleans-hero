using TheCodeKitchen.Application.Business.Helpers;
using TheCodeKitchen.Application.Contracts.Events.Cook;
using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    private async Task OnNextMomentEvent(NextMomentEvent nextMomentEvent, StreamSequenceToken _)
    {
        if (state.State.Food is not null)
            state.State.Food.Temperature = TemperatureHelper.CalculateNextMomentFoodTemperature(
                nextMomentEvent.TimePerMoment,
                state.State.Food.Temperature,
                nextMomentEvent.Temperature,
                RoomTemperatureTransferRate.Value
            );

        var timerElapsedTasks = new List<Task>();

        var activeTimers = state.State.Timers.Where(t => !t.Elapsed);

        foreach (var timer in activeTimers)
        {
            // Decrement if not already zero
            if (timer.Time > TimeSpan.Zero)
            {
                timer.Time -= nextMomentEvent.TimePerMoment;

                // Clamp to zero if it went negative
                if (timer.Time < TimeSpan.Zero)
                    timer.Time = TimeSpan.Zero;
            }

            // Check if timer has just elapsed or was already zero
            if (timer.Time > TimeSpan.Zero) continue;
            
            var @event = new TimerElapsedEvent(timer.Number, timer.Note);
            var task = realTimeCookService.SendTimerElapsedEvent(state.State.Username, @event);
            timerElapsedTasks.Add(task);
            timer.Elapsed = true;
        }

        await Task.WhenAll(timerElapsedTasks);
    }
}