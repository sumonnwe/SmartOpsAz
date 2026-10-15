using DashboardService.Application.Queries;
using DashboardService.Infrastructure.Redis;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DashboardService.Application.Handlers;

/// <summary>
/// CQRS Handler for GetDashboardSummaryQuery.
/// Aggregates metrics from Redis projections and generates operational summary.
/// </summary>
public sealed class GetDashboardSummaryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryResponse>
{
    private readonly IRedisProjectionStore _projectionStore;
    private readonly ILogger<GetDashboardSummaryHandler> _logger;

    public GetDashboardSummaryHandler(
        IRedisProjectionStore projectionStore,
        ILogger<GetDashboardSummaryHandler> logger)
    {
        _projectionStore = projectionStore ?? throw new ArgumentNullException(nameof(projectionStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DashboardSummaryResponse> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Generating dashboard summary");

            // Query projection store for aggregates
            var totalAssets = await _projectionStore.CountAllAssetsAsync(cancellationToken);
            var totalDevices = await _projectionStore.CountAllDevicesAsync(cancellationToken);

            var onlineDevices = await _projectionStore.CountDevicesByStatusAsync("Online", cancellationToken);
            var offlineDevices = await _projectionStore.CountDevicesByStatusAsync("Offline", cancellationToken);

            var assetsInUse = await _projectionStore.CountAssetsByStatusAsync("InUse", cancellationToken);
            var availableAssets = await _projectionStore.CountAssetsByStatusAsync("Available", cancellationToken);
            var assetsInMaintenance = await _projectionStore.CountAssetsByStatusAsync("Maintenance", cancellationToken);
            var retiredAssets = await _projectionStore.CountAssetsByStatusAsync("Retired", cancellationToken);

            var operationalSummary = GenerateOperationalSummary(
                totalAssets, totalDevices, onlineDevices, offlineDevices);

            var response = new DashboardSummaryResponse
            {
                TotalAssets = totalAssets,
                TotalDevices = totalDevices,
                OnlineDevices = onlineDevices,
                OfflineDevices = offlineDevices,
                AssetsInUse = assetsInUse,
                AvailableAssets = availableAssets,
                AssetsInMaintenance = assetsInMaintenance,
                RetiredAssets = retiredAssets,
                OperationalSummary = operationalSummary,
                GeneratedAtUtc = DateTimeOffset.UtcNow
            };

            _logger.LogInformation(
                "Dashboard summary generated: {TotalAssets} assets, {TotalDevices} devices, " +
                "{OnlineDevices} online, {OfflineDevices} offline",
                totalAssets, totalDevices, onlineDevices, offlineDevices);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating dashboard summary");
            throw;
        }
    }

    /// <summary>
    /// Generates a deterministic operational summary based on system health metrics.
    /// This is a fallback for future AI/OpenAI integration.
    /// </summary>
    private string GenerateOperationalSummary(
        int totalAssets,
        int totalDevices,
        int onlineDevices,
        int offlineDevices)
    {
        if (totalAssets == 0 && totalDevices == 0)
            return "System initialized but no assets or devices registered yet.";

        if (totalDevices == 0)
            return $"System has {totalAssets} assets registered. Waiting for device registration.";

        var deviceHealthPercentage = totalDevices > 0 
            ? (onlineDevices * 100) / totalDevices 
            : 0;

        return deviceHealthPercentage switch
        {
            >= 95 => $"System healthy. {onlineDevices}/{totalDevices} devices online. " +
                     $"{totalAssets} assets under management. All critical equipment monitored.",
            >= 80 => $"System operational. {onlineDevices}/{totalDevices} devices online. " +
                     $"{offlineDevices} devices offline. {totalAssets} assets managed.",
            >= 50 => $"Degraded performance. {onlineDevices}/{totalDevices} devices online. " +
                     $"{offlineDevices} offline devices require investigation.",
            _ => $"Critical alert. Only {onlineDevices}/{totalDevices} devices online. " +
                 $"{offlineDevices} devices offline. Escalate to maintenance team immediately."
        };
    }
}