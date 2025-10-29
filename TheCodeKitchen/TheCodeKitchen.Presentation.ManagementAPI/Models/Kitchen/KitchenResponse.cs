namespace TheCodeKitchen.Presentation.ManagementAPI.Models.Kitchen;

public record KitchenResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public long GameId { get; set; }
}