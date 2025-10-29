namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record AlreadyExistsError : BusinessError
{
    public AlreadyExistsError()
    {
    }

    public AlreadyExistsError(string message) : base(message)
    {
    }
}