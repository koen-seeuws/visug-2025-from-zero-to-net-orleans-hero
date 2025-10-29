using TheCodeKitchen.Application.Business.Helpers;

namespace TheCodeKitchen.Testing.UnitTests;

public sealed class TemperatureHelperTests
{
    [Theory]
    [InlineData(1, 60, 20, 100, 0.01, 56.10)] // Slow heating, 1 minute
    [InlineData(5, 60, 20, 100, 0.01, 96.02)] // Slow heating over 5 minutes
    [InlineData(1, 60, 100, 20, 0.01, 63.90)] // Slow cooling, 1 minute
    [InlineData(5, 60, 100, 20, 0.01, 23.98)] // Slow cooling over 5 minutes
    [InlineData(1, 30, 20, 100, 0.05, 82.15)] // Faster heating, short moment
    [InlineData(2, 30, 20, 100, 0.05, 96.02)] // Faster heating over 2 short moments
    [InlineData(3, 120, 50, 50, 0.1, 50.0)] // Already at equilibrium
    [InlineData(1, 60, -10, 0, 0.02, -3.01)] // Cooling from below zero
    [InlineData(10, 60, 20, 400, 0.05, 400.0)] // Long-term heating to high temp
    [InlineData(1, 60, 400, -30, 0.05, -8.59)] // Cooling from max to extreme low
    public void ShouldCorrectlyCalculateNextMomentTemperature(
        int nextMomentsPassed,
        int nextMomentDurationInSeconds,
        double currentTemperature,
        double environmentTemperature,
        double transferRate,
        double expectedNextTemperature)
    {
        // Arrange
        const double margin = 0.01;
        var timePerMoment = TimeSpan.FromSeconds(nextMomentDurationInSeconds);

        // Act
        for (var i = 0; i < nextMomentsPassed; i++)
        {
            currentTemperature = TemperatureHelper.CalculateNextMomentFoodTemperature(
                timePerMoment,
                currentTemperature,
                environmentTemperature,
                transferRate
            );
        }

        // Assert
        Assert.InRange(currentTemperature, expectedNextTemperature - margin, expectedNextTemperature + margin);
    }
}