namespace TheCodeKitchen.Application.Constants;

public static class TheCodeKitchenState
{
    // Grains
    public const string CookBook = "TheCodeKitchenCookBookState";
    public const string Cooks = "TheCodeKitchenCookState";
    public const string Equipment = "TheCodeKitchenEquipmentState";
    public const string Food = "TheCodeKitchenFoodState";
    public const string Games = "TheCodeKitchenGameState";
    public const string GameManagement = "TheCodeKitchenGameManagementState";
    public const string Kitchens = "TheCodeKitchenKitchenState";
    public const string KitchenManagement = "TheCodeKitchenKitchenManagementState";
    public const string KitchenOrders = "TheCodeKitchenKitchenOrderState";
    public const string Orders = "TheCodeKitchenOrderState";
    public const string Pantry = "TheCodeKitchenPantryState";

    public const string StreamHandles = "TheCodeKitchenStreamHandleState";

    // All Blobs
    public static readonly string[] All =
    [
        //Grains
        CookBook,
        Cooks,
        Equipment,
        Food,
        Games,
        GameManagement,
        Kitchens,
        KitchenManagement,
        KitchenOrders,
        Orders,
        Pantry,
        StreamHandles
    ];
}