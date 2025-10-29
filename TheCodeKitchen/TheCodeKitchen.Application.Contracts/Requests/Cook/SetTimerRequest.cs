namespace TheCodeKitchen.Application.Contracts.Requests.Cook;

[GenerateSerializer]
public record SetTimerRequest(TimeSpan Time, string? Note);