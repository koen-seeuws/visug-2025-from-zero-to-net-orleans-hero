using TheCodeKitchen.Application.Contracts.Requests.Cook;

namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public sealed partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> StopTimer(StopTimerRequest request)
    {
        if (!state.RecordExists)
            return new NotFoundError(
                $"The cook with username {this.GetPrimaryKeyString()} does not exist in kitchen {this.GetPrimaryKey()}");

        var timer = state.State.Timers.FirstOrDefault(t => t.Number == request.Number);

        if (timer is not null)
        {
            state.State.Timers.Remove(timer);
            await state.WriteStateAsync();
        }

        if (request.Number > state.State.TimerCounter)
            return new NotFoundError($"The timer with number {request.Number} does not exist.");

        return TheCodeKitchenUnit.Value; // The timer once existed
    }
}