namespace TheCodeKitchen.Core.Domain;

public sealed class Game(
    Guid id,
    string name,
    TimeSpan timePerMoment,
    TimeSpan minimumTimeBetweenOrders,
    TimeSpan maximumTimeBetweenOrders,
    double speedModifier = 1.0,
    short minimumItemsPerOrder = 1,
    short maximumItemsPerOrder = 4,
    double orderSpeedModifier = 1.0,
    double temperature = 30.0
)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public TimeSpan TimePerMoment { get; set; } = timePerMoment;
    public double SpeedModifier { get; set; } = speedModifier;
    public TimeSpan MinimumTimeBetweenOrders { get; set; } = minimumTimeBetweenOrders;
    public TimeSpan MaximumTimeBetweenOrders { get; set; } = maximumTimeBetweenOrders;
    public short MinimumItemsPerOrder { get; set; } = minimumItemsPerOrder;
    public short MaximumItemsPerOrder { get; set; } = maximumItemsPerOrder;
    public double OrderSpeedModifier { get; set; } = orderSpeedModifier;
    public double Temperature { get; set; } = temperature;
    public DateTimeOffset? Started { get; set; }
    public TimeSpan TimePassed { get; set; } = TimeSpan.Zero;
    public List<Guid> Kitchens { get; set; } = [];
    public List<long> OrderNumbers { get; set; } = [];
}