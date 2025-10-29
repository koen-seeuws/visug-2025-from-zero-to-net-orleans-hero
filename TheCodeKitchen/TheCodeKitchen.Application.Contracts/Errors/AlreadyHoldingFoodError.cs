namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record AlreadyHoldingFoodError : BusinessError
{
    public AlreadyHoldingFoodError()
    {
    }

    public AlreadyHoldingFoodError(string message) : base(message)
    {
    }
}