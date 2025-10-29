namespace TheCodeKitchen.Core.Domain;

public sealed class Recipe(string name, List<RecipeIngredient>? ingredients, List<RecipeStep>? steps)
{
    public string Name { get; set; } = name;
    public List<RecipeStep> Steps { get; set; } = steps ?? [];
    public List<RecipeIngredient> Ingredients { get; set; } = ingredients ?? [];
}