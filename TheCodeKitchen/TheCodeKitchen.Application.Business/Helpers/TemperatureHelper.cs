namespace TheCodeKitchen.Application.Business.Helpers;

public static class TemperatureHelper
{
    public static double CalculateNextMomentFoodTemperature(TimeSpan timePerMoment, double currentTemperature,
        double environmentTemperature, double temperatureTransferRate)
    {
        // Exponential approach to equilibrium
        var decayFactor = Math.Exp(-temperatureTransferRate * timePerMoment.TotalSeconds);
        var newTemperature = environmentTemperature + (currentTemperature - environmentTemperature) * decayFactor;

        // Clamp to realistic bounds
        return Math.Clamp(newTemperature, -30, 400);
    }
}