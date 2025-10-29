namespace TheCodeKitchen.Application.Business.Grains.CookGrain;

public partial class CookGrain
{
    public async Task<Result<TheCodeKitchenUnit>> Reset()
    {
        state.State.Food = null;
        state.State.MessageCounter = 0;
        state.State.Messages.Clear();
        state.State.TimerCounter = 0;
        state.State.Timers.Clear();
        await state.WriteStateAsync();
        return TheCodeKitchenUnit.Value;
    }
}