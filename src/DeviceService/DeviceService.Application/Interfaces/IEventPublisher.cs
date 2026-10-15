namespace DeviceService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishDeviceOfflineAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        int offlineThresholdSeconds,
        Guid correlationId,
        CancellationToken cancellationToken = default);

    Task PublishDeviceRecoveredAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default);

    Task PublishDeviceRegisteredAsync(
        Guid deviceId,
        string deviceCode,
        string deviceName,
        string deviceType,
        Guid? linkedAssetId,
        string? firmwareVersion,
        DateTimeOffset registeredAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default);

    Task PublishDeviceHeartbeatReceivedAsync(
        Guid deviceId,
        string deviceCode,
        string status,
        decimal? batteryPercentage,
        int? signalStrengthRssi,
        decimal? cpuUsagePercentage,
        decimal? memoryUsagePercentage,
        string? firmwareVersion,
        DateTimeOffset receivedAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default);
}