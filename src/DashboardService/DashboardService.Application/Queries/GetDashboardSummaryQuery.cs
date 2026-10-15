using MediatR;

namespace DashboardService.Application.Queries;

/// <summary>
/// CQRS Query for retrieving the dashboard summary.
/// Returns aggregated metrics from Redis projections.
/// </summary>
public sealed record GetDashboardSummaryQuery : IRequest<DashboardSummaryResponse>;

/// <summary>
/// Dashboard summary response containing key operational metrics.
/// All values are sourced from Redis projections updated by event consumers.
/// </summary>
public sealed record DashboardSummaryResponse
{
    /// <summary>
    /// Total number of inventory assets registered in the system.
    /// </summary>
    public int TotalAssets { get; init; }

    /// <summary>
    /// Total number of IoT devices registered in the system.
    /// </summary>
    public int TotalDevices { get; init; }

    /// <summary>
    /// Number of devices currently online.
    /// </summary>
    public int OnlineDevices { get; init; }

    /// <summary>
    /// Number of devices currently offline.
    /// </summary>
    public int OfflineDevices { get; init; }

    /// <summary>
    /// Number of assets currently in use.
    /// </summary>
    public int AssetsInUse { get; init; }

    /// <summary>
    /// Number of assets available for use.
    /// </summary>
    public int AvailableAssets { get; init; }

    /// <summary>
    /// Number of assets in maintenance.
    /// </summary>
    public int AssetsInMaintenance { get; init; }

    /// <summary>
    /// Number of retired assets (soft-deleted).
    /// </summary>
    public int RetiredAssets { get; init; }

    /// <summary>
    /// Placeholder for future alert aggregation.
    /// Currently always 0 (not yet implemented).
    /// </summary>
    public int ActiveAlerts { get; init; } = 0;

    /// <summary>
    /// Placeholder for future temperature alert aggregation.
    /// Currently always 0 (not yet implemented).
    /// </summary>
    public int TemperatureWarnings { get; init; } = 0;

    /// <summary>
    /// Placeholder for future door event aggregation.
    /// Currently always 0 (not yet implemented).
    /// </summary>
    public int TodayDoorEvents { get; init; } = 0;

    /// <summary>
    /// AI-generated or deterministic operational summary.
    /// </summary>
    public string OperationalSummary { get; init; } = string.Empty;

    /// <summary>
    /// UTC timestamp when this summary was generated.
    /// </summary>
    public DateTimeOffset GeneratedAtUtc { get; init; } = DateTimeOffset.UtcNow;
}