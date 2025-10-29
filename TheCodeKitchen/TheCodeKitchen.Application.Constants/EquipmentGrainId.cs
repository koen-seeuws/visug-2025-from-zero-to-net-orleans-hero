namespace TheCodeKitchen.Application.Constants;

public static class EquipmentGrainId
{
    public static string Create(string equipmentType, int number)
    {
        return $"{equipmentType}+{number}";
    }
}