using Externalscaler;
using FluentValidation;
using Grpc.Core;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Constants;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Extensions;
using TheCodeKitchen.Infrastructure.Orleans.Scaler.Validation;

namespace TheCodeKitchen.Infrastructure.Orleans.Scaler.Services;

public sealed class ExternalScalerService(
    IClusterClient clusterClient,
    ScaledObjectRefValidator scaledObjectRefValidator,
    ILogger<ExternalScalerService> logger
) : ExternalScaler.ExternalScalerBase
{
    private readonly IManagementGrain _managementGrain = clusterClient.GetGrain<IManagementGrain>(0);

    public override async Task<GetMetricsResponse> GetMetrics(GetMetricsRequest request, ServerCallContext context)
    {
        logger.LogInformation("Received GetMetrics request: {@request}", request);

        await ValidateRequestMetadata(request.ScaledObjectRef);

        var siloNameFilter = request.ScaledObjectRef.GetSiloNameFilter();

        var metricValue = await GetAverageSiloGrainCount(siloNameFilter);

        var response = new GetMetricsResponse();

        response.MetricValues.Add(new MetricValue
        {
            MetricName = OrleansScalerConstants.AverageGrainCountMetricName,
            MetricValue_ = metricValue
        });

        logger.LogInformation("GetMetrics response: {@response}", response);

        return response;
    }

    public override async Task<GetMetricSpecResponse> GetMetricSpec(ScaledObjectRef request, ServerCallContext context)
    {
        logger.LogInformation("Received GetMetricSpec request: {@request}", request);

        await ValidateRequestMetadata(request);

        var metricThreshold = request.GetAverageGrainCountPerSilo();

        var response = new GetMetricSpecResponse();

        response.MetricSpecs.Add(new MetricSpec
        {
            MetricName = OrleansScalerConstants.AverageGrainCountMetricName,
            TargetSize = metricThreshold
        });

        logger.LogInformation("GetMetricSpec response: {@response}", response);

        return response;
    }

    public override async Task<IsActiveResponse> IsActive(ScaledObjectRef request, ServerCallContext context)
    {
        logger.LogInformation("Received IsActive request: {@request}", request);

        await ValidateRequestMetadata(request);

        var siloNameFilter = request.GetSiloNameFilter();

        var metricValue = await GetAverageSiloGrainCount(siloNameFilter);
        var metricThreshold = request.GetAverageGrainCountPerSilo();

        var isActiveResponse = new IsActiveResponse
        {
            Result = metricValue >= metricThreshold
        };

        logger.LogInformation("IsActive response: {@isActiveResponse}", isActiveResponse);

        return isActiveResponse;
    }

    public override async Task StreamIsActive(ScaledObjectRef request,
        IServerStreamWriter<IsActiveResponse> responseStream, ServerCallContext context)
    {
        logger.LogInformation("Received StreamIsActive request: {@request}", request);

        await ValidateRequestMetadata(request);

        var siloNameFilter = request.GetSiloNameFilter();

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var metricValue = await GetAverageSiloGrainCount(siloNameFilter);
            var metricThreshold = request.GetAverageGrainCountPerSilo();

            await responseStream.WriteAsync(new IsActiveResponse
            {
                Result = metricValue >= metricThreshold
            });

            await Task.Delay(TimeSpan.FromSeconds(30), context.CancellationToken);
        }
    }

    private async Task ValidateRequestMetadata(ScaledObjectRef request)
    {
        var context = new ValidationContext<ScaledObjectRef>(request);
        var result = await scaledObjectRefValidator.ValidateAsync(context);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }

    private async Task<int> GetAverageSiloGrainCount(string siloNameFilter)
    {
        logger.LogInformation("Calculating average silo grain count: {siloNameFilter}", siloNameFilter);

        try
        {
            siloNameFilter = siloNameFilter.Trim().ToLower();
            var grainStatistics = await _managementGrain.GetSimpleGrainStatistics();
            var silos = await _managementGrain.GetDetailedHosts();

            var activeSiloAddresses = silos
                .Where(silo => silo.Status == SiloStatus.Active)
                .Where(silo => silo.HostName.Trim().ToLower().Contains(siloNameFilter))
                .Select(silo => silo.SiloAddress)
                .ToList();

            var totalGrainCount = grainStatistics
                .Where(statistic => activeSiloAddresses.Contains(statistic.SiloAddress))
                .Select(statistic => statistic.ActivationCount)
                .Sum();

            var averageGrainCountPerSilo = totalGrainCount / activeSiloAddresses.Count;

            logger.LogInformation("Calculated average silo grain count: {siloNameFilter} - {highestSiloGrainCount}",
                siloNameFilter, averageGrainCountPerSilo);

            return averageGrainCountPerSilo;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while getting average silo grain count: {message}", ex.Message);
            throw;
        }
    }
}