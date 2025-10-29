namespace TheCodeKitchen.Application.Contracts.Response.Cook;

[GenerateSerializer]
public record GetTimerResponse(int Number, TimeSpan Time, bool Elapsed, string? Note);