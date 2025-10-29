using FluentAssertions;
using TheCodeKitchen.Application.Business.Helpers;
using TheCodeKitchen.Application.Constants;
using TheCodeKitchen.Core.Domain;

namespace TheCodeKitchen.Testing.UnitTests;

public class RecipeStepRatingTests
{
    [Theory]
    [InlineData(10, 10, 1.0)]   // perfect match
    [InlineData(10, 11, 1.0)]   // within margin
    [InlineData(10, 12, 0.92)]  // 2 min over → quadratic decay
    [InlineData(10, 13, 0.82)]  // 3 min over
    [InlineData(10, 14, 0.72)]  // 4 min over
    [InlineData(10, 15, 0.5)]   // 5 min over → timing score = 0
    [InlineData(10, 20, 0.5)]   // 10 min over → still 0.5 for equipment
    public void RateSteps_ShouldApplySplitScoringModel(double expectedMinutes, double actualMinutes, double expectedScore)
    {
        var expectedSteps = new List<RecipeStep>
        {
            new(EquipmentType.Bbq, TimeSpan.FromMinutes(expectedMinutes))
        };

        var executedSteps = new List<RecipeStep>
        {
            new(EquipmentType.Bbq, TimeSpan.FromMinutes(actualMinutes))
        };

        var score = RatingHelper.RateSteps(executedSteps, expectedSteps);

        score.Should().BeApproximately(expectedScore, 0.05);
    }

    [Fact]
    public void RateSteps_ShouldReturnZero_WhenEquipmentDoesNotMatch()
    {
        var expectedSteps = new List<RecipeStep>
        {
            new(EquipmentType.Oven, TimeSpan.FromMinutes(10))
        };

        var executedSteps = new List<RecipeStep>
        {
            new(EquipmentType.Bbq, TimeSpan.FromMinutes(10))
        };

        var score = RatingHelper.RateSteps(executedSteps, expectedSteps);

        score.Should().Be(0.0);
    }

    [Fact]
    public void RateSteps_ShouldReturnOne_WhenBothExpectedAndExecutedAreEmpty()
    {
        var expectedSteps = new List<RecipeStep>();
        var executedSteps = new List<RecipeStep>();

        var score = RatingHelper.RateSteps(executedSteps, expectedSteps);

        score.Should().Be(1.0);
    }

    [Fact]
    public void RateSteps_ShouldReturnZero_WhenExecutedStepsExistButExpectedIsEmpty()
    {
        var expectedSteps = new List<RecipeStep>();
        var executedSteps = new List<RecipeStep>
        {
            new(EquipmentType.Bbq, TimeSpan.FromMinutes(10))
        };

        var score = RatingHelper.RateSteps(executedSteps, expectedSteps);

        score.Should().Be(0.0);
    }
}