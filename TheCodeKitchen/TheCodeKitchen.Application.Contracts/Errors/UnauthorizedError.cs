namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record UnauthorizedError : Error
{
    public UnauthorizedError()
    {
    }

    public UnauthorizedError(string message) : base(message)
    {
    }
}