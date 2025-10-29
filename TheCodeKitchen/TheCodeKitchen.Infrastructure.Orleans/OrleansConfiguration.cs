using FluentValidation;

namespace TheCodeKitchen.Infrastructure.Orleans;

public sealed class OrleansConfiguration
{
    public string? ClusterId { get; set; }
    public string? ServiceId { get; set; }
    public string? StateBlobContainer { get; set; }
    public StreamingEventHubConfiguration? Streaming { get; set; }
}

public sealed class OrleansConfigurationValidator : AbstractValidator<OrleansConfiguration>
{
    public OrleansConfigurationValidator()
    {
        RuleFor(x => x.ClusterId).NotEmpty();
        RuleFor(x => x.ServiceId).NotEmpty();
        RuleFor(x => x.StateBlobContainer).NotEmpty();
        RuleFor(x => x.Streaming)
            .NotEmpty()
            .SetValidator(new StreamingEventHubConfigurationValidator()!);
    }
}

public sealed class StreamingEventHubConfiguration
{
    public string? EventHub { get; set; }
    public string? ConsumerGroup { get; set; }
}

public sealed class StreamingEventHubConfigurationValidator : AbstractValidator<StreamingEventHubConfiguration>
{
    public StreamingEventHubConfigurationValidator()
    {
        RuleFor(x => x.EventHub).NotEmpty();
        RuleFor(x => x.ConsumerGroup).NotEmpty();
    }
}