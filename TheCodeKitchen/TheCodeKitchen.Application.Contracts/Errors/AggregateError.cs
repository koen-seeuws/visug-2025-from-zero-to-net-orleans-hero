namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record AggregateError : Error
{
    public AggregateError(ICollection<Error> errors)
    {
        Errors = errors;
    }

    public AggregateError(string message, ICollection<Error> errors) : base(message)
    {
        Errors = errors;
    }

    [Id(0)]
    public ICollection<Error> Errors { get; set; }
}