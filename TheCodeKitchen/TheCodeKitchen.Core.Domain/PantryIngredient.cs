namespace TheCodeKitchen.Core.Domain;

public sealed class PantryIngredient(string name)
{
    public string Name { get; set; } = name;
}