namespace TheCodeKitchen.Application.Contracts.Results;

[GenerateSerializer]
public abstract record Error
{
    public Error()
    {
        Message = string.Empty;
    }

    public Error(string message)
    {
        Message = message;
    }

    [Id(0)] public string Message { get; set; }
}