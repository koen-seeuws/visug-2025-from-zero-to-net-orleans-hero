using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Realtime;

namespace TheCodeKitchen.Application.Business.Grains.KitchenOrderGrain;

public sealed class KitchenOrderGrainStreamSubscriptionHandles
{
    public StreamSubscriptionHandle<NextMomentEvent>? NextMomentStreamSubscriptionHandle { get; set; }
}

public sealed partial class KitchenOrderGrain(
    [PersistentState(TheCodeKitchenState.KitchenOrders, TheCodeKitchenState.KitchenOrders)]
    IPersistentState<KitchenOrder> state,
    [PersistentState(TheCodeKitchenState.StreamHandles, TheCodeKitchenState.StreamHandles)]
    IPersistentState<KitchenOrderGrainStreamSubscriptionHandles> streamHandles,
    IMapper mapper,
    IRealTimeKitchenOrderService realTimeKitchenOrderService
) : Grain, IKitchenOrderGrain;