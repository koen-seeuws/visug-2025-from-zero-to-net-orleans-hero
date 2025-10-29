using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    private TimeSpan _timeUntilNewOrder = TimeSpan.Zero;

    public async Task<Result<TheCodeKitchenUnit>> NextMoment()
    {
        var gameId = this.GetPrimaryKey();

        state.State.TimePassed += state.State.TimePerMoment;
        
        if (_timeUntilNewOrder > TimeSpan.Zero)
            _timeUntilNewOrder -= state.State.TimePerMoment;
        
        // Sending out event
        var streamProvider = this.GetStreamProvider(TheCodeKitchenStreams.DefaultTheCodeKitchenProvider);
        var stream = streamProvider.GetStream<NextMomentEvent>(nameof(NextMomentEvent), gameId);
        var nextMomentEvent = new NextMomentEvent(
            state.State.Temperature,
            state.State.TimePerMoment,
            state.State.TimePassed,
            _nextMomentDelay
        );
        await stream.OnNextAsync(nextMomentEvent);

        var momentPassedEvent = new MomentPassedEvent(state.State.TimePassed);
        await realTimeGameService.SendMomentPassedEvent(state.State.Id, momentPassedEvent);

        // Order generation
        if (_timeUntilNewOrder <= TimeSpan.Zero)
        {
            _timeUntilNewOrder = TimeSpan.Zero;
            await GenerateOrder();
        }

        // Keep grain active while game is playing
        DelayDeactivation(state.State.TimePerMoment * 2);

        return TheCodeKitchenUnit.Value;
    }
}