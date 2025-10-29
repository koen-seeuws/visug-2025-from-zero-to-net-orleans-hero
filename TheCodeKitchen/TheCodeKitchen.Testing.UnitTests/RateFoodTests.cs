using FluentAssertions;
using TheCodeKitchen.Application.Business.Helpers;
using TheCodeKitchen.Core.Domain;

public class RateFoodTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid KitchenId = Guid.NewGuid();

    [Fact]
    public void RateFood_ShouldReturnZero_WhenRecipeIsMissing()
    {
        List<RecipeStep> executedSteps = [];
        List<Food> ingredients = [];
        List<Recipe> recipes = [];

        var result = RatingHelper.RateFood("NonExistent", executedSteps, ingredients, recipes);
        result.Should().Be(0.0);
    }

    [Fact]
    public void RateFood_ShouldRateSimpleRecipeWithoutIngredients()
    {
        List<RecipeStep> recipeSteps = [new("EquipmentType.Stove", TimeSpan.FromMinutes(5))];

        var recipe = new Recipe("GrilledCheese", [], recipeSteps);

        List<RecipeStep> executedSteps = [new("EquipmentType.Stove", TimeSpan.FromMinutes(5))];

        var food = new Food("GrilledCheese", 60.0, GameId, KitchenId)
        {
            Steps = executedSteps
        };

        List<Recipe> recipes = [recipe];

        var result = RatingHelper.RateFood(food.Name, food.Steps, food.Ingredients, recipes);
        result.Should().BeApproximately(1.0, 0.01);
    }

    [Fact]
    public void RateFood_ShouldRateRecipeWithOneIngredient()
    {
        List<RecipeStep> butterSteps = [new("EquipmentType.CuttingBoard", TimeSpan.FromMinutes(1))];

        var butterRecipe = new Recipe("Butter", [], butterSteps);

        List<RecipeStep> toastSteps = [new("EquipmentType.Toaster", TimeSpan.FromMinutes(2))];

        List<RecipeIngredient> toastIngredients = [new("Butter", butterSteps)];

        var toastRecipe = new Recipe("ToastWithButter", toastIngredients, toastSteps);

        var butterFood = new Food("Butter", 5.0, GameId, KitchenId)
        {
            Steps = butterSteps
        };

        var toastFood = new Food("ToastWithButter", 60.0, GameId, KitchenId, [butterFood])
        {
            Steps = toastSteps
        };

        List<Recipe> recipes = [toastRecipe, butterRecipe];

        var result = RatingHelper.RateFood(toastFood.Name, toastFood.Steps, toastFood.Ingredients, recipes);
        result.Should().BeApproximately(1.0, 0.01);
    }

    [Fact]
    public void RateFood_ShouldHandleMultipleIngredientsWithSameName()
    {
        List<RecipeStep> tomatoSteps = [new("EquipmentType.CuttingBoard", TimeSpan.FromMinutes(1))];

        List<RecipeIngredient> saladIngredients =
        [
            new("Tomato", tomatoSteps),
            new("Tomato", tomatoSteps)
        ];

        List<RecipeStep> saladSteps = [new("EquipmentType.Bowl", TimeSpan.FromMinutes(1))];

        var saladRecipe = new Recipe("DoubleTomatoSalad", saladIngredients, saladSteps);

        var tomato1 = new Food("Tomato", 20.0, GameId, KitchenId)
        {
            Steps = tomatoSteps
        };

        var tomato2 = new Food("Tomato", 20.0, GameId, KitchenId)
        {
            Steps = tomatoSteps
        };

        var saladFood = new Food("DoubleTomatoSalad", 10.0, GameId, KitchenId, [tomato1, tomato2])
        {
            Steps = saladSteps
        };

        List<Recipe> recipes = [saladRecipe];

        var result = RatingHelper.RateFood(saladFood.Name, saladFood.Steps, saladFood.Ingredients, recipes);
        result.Should().BeApproximately(1.0, 0.01);
    }
}