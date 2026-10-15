namespace SmartOpsAz.IntegrationEvents.Events;

/// <summary>
/// Integration event published when an IoT device is successfully registered.
/// Event Name: device.device-registered.v1
/// Producers: DeviceService
/// Consumers: DashboardService, future analytics services
/// </summary>
public sealed class DeviceRegisteredV1 : IntegrationEventBase
{
    public DeviceRegisteredV1()
    {
        EventType = "device.device-registered.v1";
        Producer = "DeviceService";
    }

    /// <summary>
    /// Unique identifier for the registered device.
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// Device code/serial identifier.
    /// </summary>
    public string DeviceCode { get; set; } = null!;

    /// <summary>
    /// Human-readable device name.
    /// </summary>
    public string DeviceName { get; set; } = null!;

    /// <summary>
    /// Type of device (e.g., "Sensor", "Monitor", "Gateway").
    /// </summary>
    public string DeviceType { get; set; } = null!;

    /// <summary>
    /// Inventory asset ID this device is linked to, if any.
    /// </summary>
    public Guid? LinkedAssetId { get; set; }

    /// <summary>
    /// Optional firmware version at registration time.
    /// </summary>
    public string? FirmwareVersion { get; set; }

    /// <summary>
    /// Timestamp when the device was registered.
    /// </summary>
    public DateTimeOffset RegisteredAtUtc { get; set; }
}