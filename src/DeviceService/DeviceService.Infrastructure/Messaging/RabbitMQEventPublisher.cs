using DeviceService.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using SmartOpsAz.IntegrationEvents.Events;

namespace DeviceService.Infrastructure.Messaging;

/// <summary>
/// Production event publisher that publishes domain events to RabbitMQ via MassTransit.
/// Replaces NoOpEventPublisher when RabbitMQ integration is enabled.
/// </summary>
public sealed class RabbitMQEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RabbitMQEventPublisher> _logger;

    public RabbitMQEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishDeviceOfflineAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        int offlineThresholdSeconds,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var @event = new DeviceOfflineV1
            {
                EventId = Guid.NewGuid(),
                EventType = "device.device-offline.v1",
                EventVersion = 1,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId,
                DeviceId = deviceId,
                DeviceCode = deviceCode,
                LinkedAssetId = linkedAssetId,
                LastHeartbeatAtUtc = lastHeartbeatAtUtc,
                OfflineThresholdSeconds = offlineThresholdSeconds,
                DetectedAtUtc = DateTimeOffset.UtcNow
            };

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Published DeviceOfflineV1 event. DeviceId={DeviceId}, Code={Code}, CorrelationId={CorrelationId}",
                deviceId, deviceCode, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish DeviceOfflineV1 event. DeviceId={DeviceId}, Code={Code}",
                deviceId, deviceCode);
            throw;
        }
    }

    public async Task PublishDeviceRecoveredAsync(
        Guid deviceId,
        string deviceCode,
        Guid? linkedAssetId,
        DateTimeOffset lastHeartbeatAtUtc,
        Guid correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var @event = new DeviceRecoveredV1
            {
                EventId = Guid.NewGuid(),
                EventType = "device.device-recovered.v1",
                EventVersion = 1,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId,
                DeviceId = deviceId,
                DeviceCode = deviceCode,
                LinkedAssetId = linkedAssetId,
                RecoveredAtUtc = DateTimeOffset.UtcNow
            };

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Published DeviceRecoveredV1 event. DeviceId={DeviceId}, Code={Code}, CorrelationId={CorrelationId}",
                deviceId, deviceCode, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish DeviceRecoveredV1 event. DeviceId={DeviceId}, Code={Code}",
                deviceId, deviceCode);
            throw;
        }
    }

    /// <summary>
    /// Publishes a device registration event.
    /// Called when a new device is registered.
    /// </summary>
    public async Task PublishDeviceRegisteredAsync(
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
        try
        {
            var @event = new DeviceRegisteredV1
            {
                EventId = Guid.NewGuid(),
                EventType = "device.device-registered.v1",
                EventVersion = 1,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId,
                DeviceId = deviceId,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                DeviceType = deviceType,
                LinkedAssetId = linkedAssetId,
                FirmwareVersion = firmwareVersion,
                RegisteredAtUtc = registeredAtUtc
            };

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogInformation(
                "Published DeviceRegisteredV1 event. DeviceId={DeviceId}, Code={Code}, Name={Name}, CorrelationId={CorrelationId}",
                deviceId, deviceCode, deviceName, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish DeviceRegisteredV1 event. DeviceId={DeviceId}, Code={Code}",
                deviceId, deviceCode);
            throw;
        }
    }

    /// <summary>
    /// Publishes a device heartbeat received event.
    /// Called after processing a valid heartbeat.
    /// </summary>
    public async Task PublishDeviceHeartbeatReceivedAsync(
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
        try
        {
            var @event = new DeviceHeartbeatReceivedV1
            {
                EventId = Guid.NewGuid(),
                EventType = "device.heartbeat-received.v1",
                EventVersion = 1,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                CorrelationId = correlationId,
                DeviceId = deviceId,
                DeviceCode = deviceCode,
                Status = status,
                BatteryPercentage = batteryPercentage,
                SignalStrengthRssi = signalStrengthRssi,
                CpuUsagePercentage = cpuUsagePercentage,
                MemoryUsagePercentage = memoryUsagePercentage,
                FirmwareVersion = firmwareVersion,
                ReceivedAtUtc = receivedAtUtc
            };

            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogDebug(
                "Published DeviceHeartbeatReceivedV1 event. DeviceId={DeviceId}, Code={Code}, Status={Status}, CorrelationId={CorrelationId}",
                deviceId, deviceCode, status, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish DeviceHeartbeatReceivedV1 event. DeviceId={DeviceId}, Code={Code}",
                deviceId, deviceCode);
            throw;
        }
    }
}