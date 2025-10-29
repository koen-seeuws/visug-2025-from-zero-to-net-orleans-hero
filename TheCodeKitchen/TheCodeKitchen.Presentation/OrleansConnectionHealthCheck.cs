using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TheCodeKitchen.Presentation;

public class OrleansConnectionHealthCheck(IClusterClient clusterClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = new()
    )
    {
        try
        {
            var grain = clusterClient.GetGrain<IManagementGrain>(0);
            await grain.GetHosts();
            return HealthCheckResult.Healthy("Orleans cluster is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Orleans cluster is not reachable.", ex);
        }
    }
}