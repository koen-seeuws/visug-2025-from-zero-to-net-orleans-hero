namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record NotHoldingFoodError : BusinessError
{
    public NotHoldingFoodError()
    {
    }

    public NotHoldingFoodError(string message) : base(message)
    {
    }
}