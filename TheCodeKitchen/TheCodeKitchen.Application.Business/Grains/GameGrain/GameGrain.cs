using TheCodeKitchen.Application.Contracts.Realtime;

namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain(
    [PersistentState(TheCodeKitchenState.Games, TheCodeKitchenState.Games)]
    IPersistentState<Game> state,
    IMapper mapper,
    IRealTimeGameService realTimeGameService
) : Grain, IGameGrain, IRemindable
{
    private TimeSpan? _nextMomentDelay;
    private IGrainTimer? _nextMomentTimer;
}