using DeviceService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DeviceService.Infrastructure.Messaging;

/// <summary>
/// No-operation event publisher for testing and development.
/// Used when RabbitMQ is disabled.
/// Logs events but does not publish them.
/// </summary>
public sealed class NoOpEventPublisher : IEventPublisher
{
    private readonly ILogger<NoOpEventPublisher> _logger;

    public NoOpEventPublisher(ILogger<NoOpEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishDeviceOfflineAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        int offlineThresholdSeconds,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "NoOp: DeviceOfflineV1 event would be published. DeviceId={DeviceId}, Code={Code}",
            deviceId, deviceCode);
        return Task.CompletedTask;
    }

    public Task PublishDeviceRecoveredAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "NoOp: DeviceRecoveredV1 event would be published. DeviceId={DeviceId}, Code={Code}",
            deviceId, deviceCode);
        return Task.CompletedTask;
    }

    public Task PublishDeviceRegisteredAsync(
        Guid deviceId,
        string deviceCode,
        string deviceName,
        string deviceType,
        Guid? linkedAssetId,
        string? firmwareVersion,
        DateTimeOffset registeredAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "NoOp: DeviceRegisteredV1 event would be published. DeviceId={DeviceId}, Code={Code}, Name={Name}",
            deviceId, deviceCode, deviceName);
        return Task.CompletedTask;
    }

    public Task PublishDeviceHeartbeatReceivedAsync(
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
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "NoOp: DeviceHeartbeatReceivedV1 event would be published. DeviceId={DeviceId}, Code={Code}, Status={Status}",
            deviceId, deviceCode, status);
        return Task.CompletedTask;
    }
}