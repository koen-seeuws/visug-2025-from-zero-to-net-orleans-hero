namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record InvalidJoinCodeError : BusinessError
{
    public InvalidJoinCodeError()
    {
    }

    public InvalidJoinCodeError(string message) : base(message)
    {
    }
}