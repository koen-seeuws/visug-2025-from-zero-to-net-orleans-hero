namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record BusinessError : Error
{
    public BusinessError()
    {
        
    }

    public BusinessError(string message) : base(message)
    {
    }
}