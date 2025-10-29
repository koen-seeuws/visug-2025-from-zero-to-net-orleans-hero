namespace TheCodeKitchen.Presentation.ManagementAPI.Models.Game;

public record GameResponse
{
    public long Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset? Paused { get; set; }
}