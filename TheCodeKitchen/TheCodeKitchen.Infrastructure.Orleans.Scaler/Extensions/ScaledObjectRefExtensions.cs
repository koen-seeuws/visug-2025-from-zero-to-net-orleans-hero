using Externalscaler;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Constants;

namespace TheCodeKitchen.Infrastructure.Orleans.Scaler.Extensions;

public static class ScaledObjectRefExtensions
{
    public static long GetAverageGrainCountPerSilo(this ScaledObjectRef scaledObjectRef)
    {
        if (scaledObjectRef.ScalerMetadata != null &&
            scaledObjectRef.ScalerMetadata.TryGetValue(OrleansScalerConstants.AverageGrainCountPerSiloMetadataKey, out var averageGrainCountPerSiloString) &&
            long.TryParse(averageGrainCountPerSiloString, out var averageGrainCountPerSilo))
        {
            return averageGrainCountPerSilo;
        }

        throw new InvalidOperationException($"ScaledObjectRef is missing required metadata key {OrleansScalerConstants.AverageGrainCountPerSiloMetadataKey}");
    }

    public static string GetSiloNameFilter(this ScaledObjectRef scaledObjectRef)
    {
        if (scaledObjectRef.ScalerMetadata != null &&
            scaledObjectRef.ScalerMetadata.TryGetValue(OrleansScalerConstants.SiloNameFilterMetadataKey, out var siloNameFilter) &&
            !string.IsNullOrWhiteSpace(siloNameFilter))
        {
            return siloNameFilter.Trim();
        }

        throw new InvalidOperationException($"ScaledObjectRef is missing required metadata key {OrleansScalerConstants.SiloNameFilterMetadataKey}");
    }
}