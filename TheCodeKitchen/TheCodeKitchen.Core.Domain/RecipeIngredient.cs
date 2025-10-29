namespace TheCodeKitchen.Core.Domain;

public sealed class RecipeIngredient(string name, List<RecipeStep> steps)
{
    public string Name { get; set; } = name;
    public List<RecipeStep> Steps { get; set; } = steps;
}