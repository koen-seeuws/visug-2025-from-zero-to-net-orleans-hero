using TheCodeKitchen.Application.Contracts.Realtime;

namespace TheCodeKitchen.Application.Business.Grains.GameManagementGrain;

public sealed class GameManagementState
{
    public ICollection<Guid> Games { get; set; } = new List<Guid>();
}

public sealed partial class GameManagementGrain(
    [PersistentState(TheCodeKitchenState.GameManagement, TheCodeKitchenState.GameManagement)]
    IPersistentState<GameManagementState> state,
    IRealTimeGameManagementService realTimeGameManagementService
) : Grain, IGameManagementGrain;