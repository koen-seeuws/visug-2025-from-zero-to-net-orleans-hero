namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record NotFoundError : Error
{
    public NotFoundError()
    {
    }

    public NotFoundError(string message) : base(message)
    {
    }
}