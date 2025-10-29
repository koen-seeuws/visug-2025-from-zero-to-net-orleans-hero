namespace TheCodeKitchen.Application.Constants;

public static class EquipmentType
{
    public const string Bbq = "BBQ";
    public const string Blender = "Blender";
    public const string Counter = "Counter";
    public const string CuttingBoard = "Cutting Board";
    public const string Fridge = "Fridge";
    public const string Freezer = "Freezer";
    public const string Stove = "Stove";
    public const string Fryer = "Fryer";
    public const string HotPlate = "Hot Plate";
    public const string Mixer = "Mixer";
    public const string Oven = "Oven";

    public static readonly string[] All =
    [
        Bbq,
        Blender,
        Counter,
        CuttingBoard,
        Fridge,
        Freezer,
        Fryer,
        HotPlate,
        Mixer,
        Oven,
        Stove
    ];

    public static readonly string[] Steppable =
    [
        Bbq,
        Blender,
        CuttingBoard,
        Fridge,
        Freezer,
        Fryer,
        Mixer,
        Oven,
        Stove
    ];
}