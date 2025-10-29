using Microsoft.Extensions.Logging;

namespace TheCodeKitchen.Application.Business.Interceptors;

public sealed class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : IIncomingGrainCallFilter
{
    public async Task Invoke(IIncomingGrainCallContext context)
    {
        var grainType = context.Grain.GetType();

        if (grainType.Namespace?.StartsWith("TheCodeKitchen") ?? false)
        {
            // Only log calls to grains in our application namespace
            var grainTypeName = grainType.Name;
            var methodName = context.InterfaceMethod.Name;
            var grainId = context.TargetId.Key.ToString();

            try
            {
                logger.LogInformation("Invoking {grainTypeName}.{methodName} ({grainId})...", grainTypeName, methodName,
                    grainId);

                await context.Invoke();

                logger.LogInformation(
                    "Completed {grainTypeName}.{methodName} ({grainId})",
                    grainTypeName, methodName, grainId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception in {grainType}.{methodName} ({grainId}): {message}",
                    grainTypeName, methodName, grainId, exception.Message);
                throw;
            }
        }
        else
        {
            // Not our application namespace, just proceed
            await context.Invoke();
        }
    }
}