namespace SmartOpsAz.IntegrationEvents.Events;

/// <summary>
/// Integration event published when a device heartbeat is received and processed.
/// Event Name: device.heartbeat-received.v1
/// Producers: DeviceService
/// Consumers: DashboardService, analytics, alerting services
/// </summary>
public sealed class DeviceHeartbeatReceivedV1 : IntegrationEventBase
{
    public DeviceHeartbeatReceivedV1()
    {
        EventType = "device.heartbeat-received.v1";
        Producer = "DeviceService";
    }

    /// <summary>
    /// Device ID sending the heartbeat.
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// Device code for reference.
    /// </summary>
    public string DeviceCode { get; set; } = null!;

    /// <summary>
    /// Device status after heartbeat processing (e.g., "Online").
    /// </summary>
    public string Status { get; set; } = "Online";

    /// <summary>
    /// Battery percentage (0-100), if available.
    /// </summary>
    public decimal? BatteryPercentage { get; set; }

    /// <summary>
    /// Signal strength in dBm (-120 to 0), if available.
    /// </summary>
    public int? SignalStrengthRssi { get; set; }

    /// <summary>
    /// CPU usage percentage (0-100), if available.
    /// </summary>
    public decimal? CpuUsagePercentage { get; set; }

    /// <summary>
    /// Memory usage percentage (0-100), if available.
    /// </summary>
    public decimal? MemoryUsagePercentage { get; set; }

    /// <summary>
    /// Firmware version reported by device, if updated.
    /// </summary>
    public string? FirmwareVersion { get; set; }

    /// <summary>
    /// Timestamp when heartbeat was received by the server.
    /// </summary>
    public DateTimeOffset ReceivedAtUtc { get; set; }
}