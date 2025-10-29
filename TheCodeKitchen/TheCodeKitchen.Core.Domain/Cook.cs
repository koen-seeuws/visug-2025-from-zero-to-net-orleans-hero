namespace TheCodeKitchen.Core.Domain;

public sealed class Cook(string username, string passwordHash, Guid game, Guid kitchen)
{
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
    public Guid Game { get; set; } = game;
    public Guid Kitchen { get; set; } = kitchen;
    public Food? Food { get; set; }
    public int MessageCounter { get; set; }
    public List<Message> Messages { get; set; } = [];
    public int TimerCounter { get; set; }
    public List<Timer> Timers { get; set; } = [];
}