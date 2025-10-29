namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record InvalidRecipeError : BusinessError
{
    public InvalidRecipeError()
    {
    }

    public InvalidRecipeError(string message) : base(message)
    {
    }
}