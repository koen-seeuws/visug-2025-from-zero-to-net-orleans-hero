namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record GameAlreadyStartedError : BusinessError
{
    public GameAlreadyStartedError()
    {
    }

    public GameAlreadyStartedError(string message) : base(message)
    {
    }
}