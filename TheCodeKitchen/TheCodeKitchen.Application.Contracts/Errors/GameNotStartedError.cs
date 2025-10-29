namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record GameNotStartedError : BusinessError
{
    public GameNotStartedError()
    {
    }

    public GameNotStartedError(string message) : base(message)
    {
    }
}