namespace TheCodeKitchen.Application.Contracts.Errors;

[GenerateSerializer]
public record EquipmentEmptyError : BusinessError
{
    public EquipmentEmptyError()
    {
    }

    public EquipmentEmptyError(string message) : base(message)
    {
    }
}