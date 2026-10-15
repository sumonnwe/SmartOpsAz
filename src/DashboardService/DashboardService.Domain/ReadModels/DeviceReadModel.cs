namespace DashboardService.Domain.ReadModels;

/// <summary>
/// Read model for IoT device data used in dashboard queries.
/// Persisted in Redis for fast access.
/// This is not a domain entity but a projection for read-side operations.
/// </summary>
public sealed record DeviceReadModel
{
    /// <summary>
    /// Unique identifier for the device.
    /// </summary>
    public Guid DeviceId { get; init; }

    /// <summary>
    /// Device code/serial identifier.
    /// </summary>
    public string DeviceCode { get; init; } = null!;

    /// <summary>
    /// Human-readable device name.
    /// </summary>
    public string DeviceName { get; init; } = null!;

    /// <summary>
    /// Device type classification.
    /// </summary>
    public string DeviceType { get; init; } = null!;

    /// <summary>
    /// Current status: Online, Offline, Maintenance, Error.
    /// </summary>
    public string Status { get; init; } = "Offline";

    /// <summary>
    /// Inventory asset ID this device is linked to, if any.
    /// </summary>
    public Guid? LinkedAssetId { get; init; }

    /// <summary>
    /// Current firmware version.
    /// </summary>
    public string? FirmwareVersion { get; init; }

    /// <summary>
    /// Battery percentage (0-100).
    /// </summary>
    public decimal? BatteryPercentage { get; init; }

    /// <summary>
    /// Signal strength in dBm (-120 to 0).
    /// </summary>
    public int? SignalStrengthRssi { get; init; }

    /// <summary>
    /// CPU usage percentage (0-100).
    /// </summary>
    public decimal? CpuUsagePercentage { get; init; }

    /// <summary>
    /// Memory usage percentage (0-100).
    /// </summary>
    public decimal? MemoryUsagePercentage { get; init; }

    /// <summary>
    /// UTC timestamp of the last heartbeat received.
    /// </summary>
    public DateTimeOffset? LastHeartbeatAtUtc { get; init; }

    /// <summary>
    /// UTC timestamp when device was registered.
    /// </summary>
    public DateTimeOffset RegisteredAtUtc { get; init; }

    /// <summary>
    /// UTC timestamp when device was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAtUtc { get; init; }

    /// <summary>
    /// Event version for idempotency and ordering.
    /// Used to detect and skip duplicate or out-of-order events.
    /// </summary>
    public long EventVersion { get; init; }
}