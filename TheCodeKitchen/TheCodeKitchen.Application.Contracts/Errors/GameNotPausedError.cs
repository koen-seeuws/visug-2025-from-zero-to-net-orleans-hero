namespace TheCodeKitchen.Application.Contracts.Errors;

public record GameNotPausedError : BusinessError
{
    public GameNotPausedError()
    {
    }

    public GameNotPausedError(string message) : base(message)
    {
    }
}