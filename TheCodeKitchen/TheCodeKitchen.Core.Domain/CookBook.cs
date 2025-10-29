namespace TheCodeKitchen.Core.Domain;

public sealed class CookBook(Guid id)
{
    public Guid Id { get; set; } = id;
    public List<Recipe> Recipes { get; set; } = [];
}