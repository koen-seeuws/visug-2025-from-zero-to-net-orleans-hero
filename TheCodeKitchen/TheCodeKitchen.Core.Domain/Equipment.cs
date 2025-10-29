namespace TheCodeKitchen.Core.Domain;

public sealed class Equipment(Guid game, Guid kitchen, string equipmentType, int number, double? temperature, double? temperatureTransferRate)
{
    public Guid Game { get; set; } = game;
    public Guid Kitchen { get; set; } = kitchen;
    public string EquipmentType { get; set; } = equipmentType;
    public int Number { get; set; } = number;
    public double? Temperature { get; set; } = temperature;
    public double? TemperatureTransferRate { get; set; } = temperatureTransferRate;
    public TimeSpan? MixtureTime { get; set; }
    public List<Food> Foods { get; set; } = [];
}