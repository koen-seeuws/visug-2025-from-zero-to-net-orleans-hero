namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record OrderAlreadyCompletedError : BusinessError
{
    public OrderAlreadyCompletedError()
    {
    }

    public OrderAlreadyCompletedError(string message) : base(message)
    {
    }
}