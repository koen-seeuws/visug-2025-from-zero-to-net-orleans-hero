namespace TheCodeKitchen.CookTemplate.Contracts.Reponses.Timer;

public record GetTimerResponse(int Number, TimeSpan Time, bool Elapsed, string Note);