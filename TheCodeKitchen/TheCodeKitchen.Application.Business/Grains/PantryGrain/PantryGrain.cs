namespace TheCodeKitchen.Application.Business.Grains.PantryGrain;

public sealed partial class PantryGrain(
    [PersistentState(TheCodeKitchenState.Pantry, TheCodeKitchenState.Pantry)]
    IPersistentState<Pantry> state,
    IMapper mapper
) : Grain, IPantryGrain;