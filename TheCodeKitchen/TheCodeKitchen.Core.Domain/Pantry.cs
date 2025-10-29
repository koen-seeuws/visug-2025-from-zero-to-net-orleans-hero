namespace TheCodeKitchen.Core.Domain;

public sealed class Pantry(Guid id, double temperature)
{
    public Guid Id { get; set; } = id;
    public double Temperature { get; set; } = temperature;
    public List<PantryIngredient> Ingredients { get; set; } = [];
}