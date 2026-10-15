using DashboardService.Domain.ReadModels;
using DashboardService.Infrastructure.Redis;
using MassTransit;
using Microsoft.Extensions.Logging;
using SmartOpsAz.IntegrationEvents.Events;

namespace DashboardService.Application.Consumers;

/// <summary>
/// Consumes DeviceHeartbeatReceivedV1 events and updates device telemetry in the read model.
/// Implements idempotent processing to handle duplicate events.
/// </summary>
public sealed class DeviceHeartbeatReceivedConsumer : IConsumer<DeviceHeartbeatReceivedV1>
{
    private readonly IRedisProjectionStore _projectionStore;
    private readonly ILogger<DeviceHeartbeatReceivedConsumer> _logger;

    public DeviceHeartbeatReceivedConsumer(
        IRedisProjectionStore projectionStore,
        ILogger<DeviceHeartbeatReceivedConsumer> logger)
    {
        _projectionStore = projectionStore ?? throw new ArgumentNullException(nameof(projectionStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeviceHeartbeatReceivedV1> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogDebug(
                "Consuming DeviceHeartbeatReceivedV1 event. DeviceId={DeviceId}, Code={Code}, Status={Status}",
                @event.DeviceId, @event.DeviceCode, @event.Status);

            // Check if device exists in read model
            var existing = await _projectionStore.GetDeviceAsync(@event.DeviceId);

            if (existing != null)
            {
                // Idempotency check: skip if we've already processed a newer event
                if (existing.EventVersion >= @event.EventVersion)
                {
                    _logger.LogDebug(
                        "Skipping out-of-order or duplicate DeviceHeartbeatReceivedV1 event. " +
                        "DeviceId={DeviceId}, CurrentVersion={CurrentVersion}, EventVersion={EventVersion}",
                        @event.DeviceId, existing.EventVersion, @event.EventVersion);
                    return;
                }

                // Update device with latest telemetry
                var updated = existing with
                {
                    Status = @event.Status,
                    BatteryPercentage = @event.BatteryPercentage ?? existing.BatteryPercentage,
                    SignalStrengthRssi = @event.SignalStrengthRssi ?? existing.SignalStrengthRssi,
                    CpuUsagePercentage = @event.CpuUsagePercentage ?? existing.CpuUsagePercentage,
                    MemoryUsagePercentage = @event.MemoryUsagePercentage ?? existing.MemoryUsagePercentage,
                    FirmwareVersion = !string.IsNullOrWhiteSpace(@event.FirmwareVersion) 
                        ? @event.FirmwareVersion 
                        : existing.FirmwareVersion,
                    LastHeartbeatAtUtc = @event.ReceivedAtUtc,
                    UpdatedAtUtc = @event.ReceivedAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(updated);

                _logger.LogDebug(
                    "Device telemetry updated. DeviceId={DeviceId}, Battery={Battery}%, RSSI={RSSI}, CPU={CPU}%, Memory={Memory}%",
                    @event.DeviceId, @event.BatteryPercentage, @event.SignalStrengthRssi,
                    @event.CpuUsagePercentage, @event.MemoryUsagePercentage);
            }
            else
            {
                // Device doesn't exist in read model yet (out-of-order event)
                _logger.LogWarning(
                    "Device not found in read model for heartbeat event. Creating entry. " +
                    "DeviceId={DeviceId}, Code={Code}",
                    @event.DeviceId, @event.DeviceCode);

                var newDevice = new DeviceReadModel
                {
                    DeviceId = @event.DeviceId,
                    DeviceCode = @event.DeviceCode,
                    DeviceName = @event.DeviceCode,
                    DeviceType = "Unknown",
                    Status = @event.Status,
                    BatteryPercentage = @event.BatteryPercentage,
                    SignalStrengthRssi = @event.SignalStrengthRssi,
                    CpuUsagePercentage = @event.CpuUsagePercentage,
                    MemoryUsagePercentage = @event.MemoryUsagePercentage,
                    FirmwareVersion = @event.FirmwareVersion,
                    LastHeartbeatAtUtc = @event.ReceivedAtUtc,
                    RegisteredAtUtc = @event.OccurredAtUtc,
                    UpdatedAtUtc = @event.ReceivedAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(newDevice);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error consuming DeviceHeartbeatReceivedV1 event. DeviceId={DeviceId}, Code={Code}",
                @event.DeviceId, @event.DeviceCode);
            throw;
        }
    }
}