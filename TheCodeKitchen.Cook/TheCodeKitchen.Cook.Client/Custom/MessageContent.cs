namespace TheCodeKitchen.Cook.Client.Custom;

public record MessageContent(
    string Code,
    long? Order,
    string? Food,
    string? EquipmentType,
    int? EquipmentNumber
);