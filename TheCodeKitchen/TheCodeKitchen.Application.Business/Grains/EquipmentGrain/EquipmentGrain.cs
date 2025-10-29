using TheCodeKitchen.Application.Contracts.Events.Game;

namespace TheCodeKitchen.Application.Business.Grains.EquipmentGrain;

public sealed class EquipmentGrainStreamSubscriptionHandles
{
    public StreamSubscriptionHandle<NextMomentEvent>? NextMomentStreamSubscriptionHandle { get; set; }
}

public sealed partial class EquipmentGrain(
    [PersistentState(TheCodeKitchenState.Equipment, TheCodeKitchenState.Equipment)]
    IPersistentState<Equipment> state,
    [PersistentState(TheCodeKitchenState.StreamHandles, TheCodeKitchenState.StreamHandles)]
    IPersistentState<EquipmentGrainStreamSubscriptionHandles> streamSubscriptionHandles,
    IMapper mapper
) : Grain, IEquipmentGrain;