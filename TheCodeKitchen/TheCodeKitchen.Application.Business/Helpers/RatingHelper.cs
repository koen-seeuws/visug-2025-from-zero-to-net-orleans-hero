namespace TheCodeKitchen.Application.Business.Helpers;

public static class RatingHelper
{
    public static double RateFood(string food, ICollection<RecipeStep> executedSteps, ICollection<Food> ingredients,
        ICollection<Recipe> recipes)
    {
        var recipe = recipes.FirstOrDefault(r => r.Name.Equals(food, StringComparison.OrdinalIgnoreCase));
        
        if (recipe is null)
            return 0.0;

        var recipeRating = RateSteps(executedSteps, recipe.Steps);

        // Build a lookup for recipe names
        var recipeNames = new HashSet<string>(recipes.Select(r => r.Name), StringComparer.OrdinalIgnoreCase);

        // Group recipe ingredients by name and track occurrences
        var recipeIngredientGroups = recipe.Ingredients
            .GroupBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        var nameCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var ingredientRatings = ingredients.Select(ingredient =>
        {
            var name = ingredient.Name;
            var occurrence = nameCounts.GetValueOrDefault(name);
            nameCounts[name] = occurrence + 1;

            var recipeMatch = recipeIngredientGroups.TryGetValue(name, out var matches) && occurrence < matches.Count
                ? matches[occurrence]
                : null;

            if (recipeNames.Contains(name))
                return RateFood(name, ingredient.Steps, ingredient.Ingredients, recipes);
            
            return recipeMatch != null
                ? RateSteps(ingredient.Steps, recipeMatch.Steps)
                : 0.0;
        });

        return ingredientRatings.Append(recipeRating).Average();
    }


    public static double RateSteps(ICollection<RecipeStep> executedSteps, ICollection<RecipeStep> expectedSteps)
    {
        var executed = executedSteps.ToArray();
        var expected = expectedSteps.ToArray();

        var m = executed.Length;
        var n = expected.Length;

        // Edge case: no expected steps
        if (n == 0)
            return m == 0 ? 1.0 : 0.0;

        // DP table: dp[i,j] = best score for first i executed and first j expected
        var dp = new double[m + 1, n + 1];

        // Initialize base cases
        for (var i = 0; i <= m; i++)
            dp[i, 0] = i * -0.5; // penalty for extra steps

        for (var j = 0; j <= n; j++)
            dp[0, j] = j * -0.5; // penalty for missing steps

        // Fill DP table
        for (var i = 1; i <= m; i++)
        {
            for (var j = 1; j <= n; j++)
            {
                var matchScore = CalculateStepScore(executed[i - 1], expected[j - 1]);

                dp[i, j] = Math.Max(
                    dp[i - 1, j - 1] + matchScore, // match or substitute
                    Math.Max(
                        dp[i - 1, j] - 0.5, // extra executed step
                        dp[i, j - 1] - 0.5 // missing expected step
                    )
                );
            }
        }

        // Normalize score
        double maxPossibleScore = Math.Max(m, n); // worst case denominator
        var finalScore = dp[m, n] / maxPossibleScore;

        // Clamp between 0 and 1
        return Math.Max(0.0, Math.Min(1.0, finalScore));
    }

    private static double CalculateStepScore(RecipeStep actual, RecipeStep expected)
    {
        const double equipmentScore = 0.5;
        const double timingScoreMax = 0.5;
        const double minTimingScore = 0.0;

        // Equipment match
        if (!actual.EquipmentType.Equals(expected.EquipmentType, StringComparison.OrdinalIgnoreCase))
            return 0.0;

        // Equipment matches → base score
        var score = equipmentScore;

        // Time match with quadratic decay
        var expectedSeconds = expected.Time.TotalSeconds;
        var actualSeconds = actual.Time.TotalSeconds;
        var margin = expectedSeconds * RatingMargin.StepTime;

        var timeDiff = Math.Abs(expectedSeconds - actualSeconds);

        if (timeDiff <= margin)
        {
            score += timingScoreMax; // full timing credit
        }
        else
        {
            var maxPenalty = expectedSeconds * 0.5; // 50% deviation range
            var penaltyRatio = Math.Min(timeDiff / maxPenalty, 1.0);
            var timingScore = timingScoreMax * (1.0 - Math.Pow(penaltyRatio, 2));

            score += Math.Max(timingScore, minTimingScore);
        }

        return score;
    }
}