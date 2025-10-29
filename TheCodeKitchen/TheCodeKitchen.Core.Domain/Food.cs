namespace TheCodeKitchen.Core.Domain;

public sealed class Food(string name, double temperature, Guid game, Guid kitchen, List<Food>? ingredients = null)
{
    public string Name { get; set; } = name;
    public double Temperature { get; set; } = temperature;
    public List<Food> Ingredients { get; set; } = ingredients ?? [];
    public List<RecipeStep> Steps { get; set; } = [];
    public Guid Game { get; set; } = game;
    public Guid Kitchen { get; set; } = kitchen;
}