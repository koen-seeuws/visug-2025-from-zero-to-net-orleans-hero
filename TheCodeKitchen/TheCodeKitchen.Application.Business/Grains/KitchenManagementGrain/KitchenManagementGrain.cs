namespace TheCodeKitchen.Application.Business.Grains.KitchenManagementGrain;

public sealed class KitchenManagementState
{
    public IDictionary<string, Guid> KitchenCodes { get; set; } = new Dictionary<string, Guid>();
}

public sealed partial class KitchenManagementGrain(
    [PersistentState(TheCodeKitchenState.KitchenManagement, TheCodeKitchenState.KitchenManagement)]
    IPersistentState<KitchenManagementState> state
) : Grain, IKitchenManagementGrain;