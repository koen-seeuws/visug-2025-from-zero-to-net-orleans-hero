namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record EmptyError : BusinessError
{
    public EmptyError()
    {
    }

    public EmptyError(string message) : base(message)
    {
    }
}