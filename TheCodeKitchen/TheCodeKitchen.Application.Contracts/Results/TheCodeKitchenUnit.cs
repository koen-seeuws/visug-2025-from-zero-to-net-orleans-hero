namespace TheCodeKitchen.Application.Contracts.Results;

[GenerateSerializer]
public readonly record struct TheCodeKitchenUnit
{
    public static TheCodeKitchenUnit Value { get; } = new();
}