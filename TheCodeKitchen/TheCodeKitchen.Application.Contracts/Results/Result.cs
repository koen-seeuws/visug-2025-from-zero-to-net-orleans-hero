using TheCodeKitchen.Application.Contracts.Errors;

namespace TheCodeKitchen.Application.Contracts.Results;

[GenerateSerializer]
public sealed class Result<T>
{
    [Id(0)] public bool Succeeded { get; set; }
    [Id(1)] public T Value { get; set; }
    [Id(2)] public Error Error { get; set; }

    public static implicit operator Result<T>(T data)
        => new()
        {
            Succeeded = true,
            Value = data
        };

    public static implicit operator Result<T>(Error error)
        => new()
        {
            Succeeded = false,
            Error = error
        };

    public TMatch Match<TMatch>(Func<T, TMatch> onSuccess, Func<Error, TMatch> onFail)
        => Succeeded ? onSuccess(Value) : onFail(Error);

    public static Result<IEnumerable<T>> Combine(IEnumerable<Result<T>> results)
    {
        var values = new List<T>();
        var errors = new List<Error>();

        foreach (var result in results)
        {
            if (result.Succeeded)
                values.Add(result.Value);
            else
                errors.Add(result.Error);
        }

        return errors.Count is 0 ? values : new AggregateError(errors);
    }
}