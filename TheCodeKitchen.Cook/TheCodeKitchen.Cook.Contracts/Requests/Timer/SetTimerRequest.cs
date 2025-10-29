namespace TheCodeKitchen.Cook.Contracts.Requests.Timer;

public record SetTimerRequest(TimeSpan Time, string Note);