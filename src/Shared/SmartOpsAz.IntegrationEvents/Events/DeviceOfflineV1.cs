namespace SmartOpsAz.IntegrationEvents.Events;

/// <summary>
/// Integration event published when a device is detected as offline.
/// Event Name: device.device-offline.v1
/// Producers: DeviceService (via DeviceOfflineDetectionWorker)
/// Consumers: DashboardService, alert service, notification service
/// 
/// Published once per offline transition when heartbeat has not been received
/// for longer than the configured threshold (default: 300 seconds).
/// </summary>
public sealed class DeviceOfflineV1 : IntegrationEventBase
{
    public DeviceOfflineV1()
    {
        EventType = "device.device-offline.v1";
        Producer = "DeviceService";
    }

    /// <summary>
    /// ID of the device detected as offline.
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// Device code for operational reference.
    /// </summary>
    public string DeviceCode { get; set; } = null!;

    /// <summary>
    /// Linked inventory asset ID, if the device is associated with an asset.
    /// </summary>
    public Guid? LinkedAssetId { get; set; }

    /// <summary>
    /// Timestamp of the last heartbeat received before offline detection.
    /// Null if no heartbeat has ever been received.
    /// </summary>
    public DateTimeOffset? LastHeartbeatAtUtc { get; set; }

    /// <summary>
    /// Configured threshold in seconds that triggered offline detection.
    /// </summary>
    public int OfflineThresholdSeconds { get; set; }

    /// <summary>
    /// Timestamp when offline detection occurred.
    /// </summary>
    public DateTimeOffset DetectedAtUtc { get; set; }
}