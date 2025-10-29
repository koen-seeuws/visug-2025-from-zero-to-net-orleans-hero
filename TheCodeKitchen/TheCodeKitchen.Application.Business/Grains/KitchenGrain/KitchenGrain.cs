using Orleans.Placement;
using TheCodeKitchen.Application.Contracts.Events.Game;
using TheCodeKitchen.Application.Contracts.Events.KitchenOrder;
using TheCodeKitchen.Application.Contracts.Realtime;

namespace TheCodeKitchen.Application.Business.Grains.KitchenGrain;

public sealed class KitchenGrainStreamSubscriptionHandles
{
    public StreamSubscriptionHandle<NextMomentEvent>? NextMomentStreamSubscriptionHandle { get; set; } = null;
    public StreamSubscriptionHandle<OrderGeneratedEvent>? OrderGeneratedStreamSubscriptionHandle { get; set; }
    public StreamSubscriptionHandle<KitchenOrderRatingUpdatedEvent>? KitchenOrderRatingUpdatedStreamSubscriptionHandle { get; set; }
}

[PreferLocalPlacement]
public sealed partial class KitchenGrain(
    [PersistentState(TheCodeKitchenState.Kitchens, TheCodeKitchenState.Kitchens)]
    IPersistentState<Kitchen> state,
    [PersistentState(TheCodeKitchenState.StreamHandles, TheCodeKitchenState.StreamHandles)]
    IPersistentState<KitchenGrainStreamSubscriptionHandles> streamHandles,
    IMapper mapper,
    IRealTimeKitchenService realTimeKitchenService,
    IRealTimeGameService realTimeGameService
) : Grain, IKitchenGrain;