namespace TheCodeKitchen.CookTemplate.Contracts.Requests.Timer;

public record SetTimerRequest(TimeSpan Time, string Note);