namespace TheCodeKitchen.Application.Contracts.Response.Game;

[GenerateSerializer]
public record PauseOrResumeGameResponse(bool Paused);