namespace SmartOpsAz.IntegrationEvents.Events;

/// <summary>
/// Integration event published when an offline device recovers online.
/// Event Name: device.device-recovered.v1
/// Producers: DeviceService (via heartbeat processing)
/// Consumers: DashboardService, alert service, notification service
/// 
/// Published once when device transitions from Offline to Online status
/// following receipt of a valid heartbeat.
/// </summary>
public sealed class DeviceRecoveredV1 : IntegrationEventBase
{
    public DeviceRecoveredV1()
    {
        EventType = "device.device-recovered.v1";
        Producer = "DeviceService";
    }

    /// <summary>
    /// ID of the device that recovered.
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// Device code for operational reference.
    /// </summary>
    public string DeviceCode { get; set; } = null!;

    /// <summary>
    /// Linked inventory asset ID, if associated.
    /// </summary>
    public Guid? LinkedAssetId { get; set; }

    /// <summary>
    /// Timestamp when recovery was detected (i.e., when valid heartbeat was received).
    /// </summary>
    public DateTimeOffset RecoveredAtUtc { get; set; }
}