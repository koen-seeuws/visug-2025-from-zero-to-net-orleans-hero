namespace TheCodeKitchen.Application.Business.Grains.GameGrain;

public sealed partial class GameGrain
{
    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        switch (reminderName)
        {
            case nameof(ResumeGame):
                await ResumeGame();
                break;
            default:
                throw new NotImplementedException();
        }
    }
}