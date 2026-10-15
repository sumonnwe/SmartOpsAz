namespace DashboardService.Domain.ReadModels;

/// <summary>
/// Read model for inventory asset data used in dashboard queries.
/// Persisted in Redis for fast access.
/// This is not a domain entity but a projection for read-side operations.
/// </summary>
public sealed record AssetReadModel
{
    /// <summary>
    /// Unique identifier for the asset.
    /// </summary>
    public Guid AssetId { get; init; }

    /// <summary>
    /// Human-readable name of the asset.
    /// </summary>
    public string AssetName { get; init; } = null!;

    /// <summary>
    /// Unique serial number for the asset.
    /// </summary>
    public string SerialNumber { get; init; } = null!;

    /// <summary>
    /// Type of asset (e.g., "Monitor", "Pump", "Bed").
    /// </summary>
    public string AssetType { get; init; } = null!;

    /// <summary>
    /// Current status: Available, InUse, Maintenance, Offline, Retired.
    /// </summary>
    public string Status { get; init; } = "Available";

    /// <summary>
    /// Department ID the asset is assigned to.
    /// </summary>
    public Guid? DepartmentId { get; init; }

    /// <summary>
    /// Location ID where the asset is currently located.
    /// </summary>
    public Guid? LocationId { get; init; }

    /// <summary>
    /// UTC timestamp when asset was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; init; }

    /// <summary>
    /// UTC timestamp when asset was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAtUtc { get; init; }

    /// <summary>
    /// Event version for idempotency and ordering.
    /// Used to detect and skip duplicate or out-of-order events.
    /// </summary>
    public long EventVersion { get; init; }
}