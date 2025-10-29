using Externalscaler;
using FluentValidation;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Constants;

namespace TheCodeKitchen.Infrastructure.Orleans.Scaler.Validation;

public sealed class ScaledObjectRefValidator : AbstractValidator<ScaledObjectRef>
{
    public ScaledObjectRefValidator()
    {
        RuleFor(x => x.ScalerMetadata)
            .NotEmpty()
            .Must(scaleMetadata =>
                scaleMetadata.TryGetValue(OrleansScalerConstants.AverageGrainCountPerSiloMetadataKey,
                    out var maxSiloCountPerSiloString) &&
                long.TryParse(maxSiloCountPerSiloString, out var maxSiloCountPerSilo) &&
                maxSiloCountPerSilo > 0
            )
            .WithMessage(
                $"You must provide a number > 0 for {OrleansScalerConstants.AverageGrainCountPerSiloMetadataKey} in ScalerMetadata")
            .Must(scaleMetadata =>
                scaleMetadata.TryGetValue(OrleansScalerConstants.SiloNameFilterMetadataKey,
                    out var siloNameFilter) &&
                !string.IsNullOrWhiteSpace(siloNameFilter))
            .WithMessage(
                $"You must provide a non-empty value for {OrleansScalerConstants.SiloNameFilterMetadataKey} in ScalerMetadata");
    }
}