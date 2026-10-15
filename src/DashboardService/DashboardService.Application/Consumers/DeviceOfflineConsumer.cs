using DashboardService.Domain.ReadModels;
using DashboardService.Infrastructure.Redis;
using MassTransit;
using Microsoft.Extensions.Logging;
using SmartOpsAz.IntegrationEvents.Events;

namespace DashboardService.Application.Consumers;

/// <summary>
/// Consumes DeviceOfflineV1 events and updates the device read model status.
/// Implements idempotent processing to handle duplicate or out-of-order events.
/// </summary>
public sealed class DeviceOfflineConsumer : IConsumer<DeviceOfflineV1>
{
    private readonly IRedisProjectionStore _projectionStore;
    private readonly ILogger<DeviceOfflineConsumer> _logger;

    public DeviceOfflineConsumer(
        IRedisProjectionStore projectionStore,
        ILogger<DeviceOfflineConsumer> logger)
    {
        _projectionStore = projectionStore ?? throw new ArgumentNullException(nameof(projectionStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeviceOfflineV1> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogInformation(
                "Consuming DeviceOfflineV1 event. DeviceId={DeviceId}, Code={Code}, EventVersion={Version}, CorrelationId={CorrelationId}",
                @event.DeviceId, @event.DeviceCode, @event.EventVersion, @event.CorrelationId);

            // Check if device exists in read model
            var existing = await _projectionStore.GetDeviceAsync(@event.DeviceId);

            if (existing != null)
            {
                // Idempotency check: skip if we've already processed this event or a newer one
                if (existing.EventVersion >= @event.EventVersion)
                {
                    _logger.LogInformation(
                        "Skipping out-of-order or duplicate DeviceOfflineV1 event. " +
                        "DeviceId={DeviceId}, CurrentVersion={CurrentVersion}, EventVersion={EventVersion}",
                        @event.DeviceId, existing.EventVersion, @event.EventVersion);
                    return;
                }

                // Update existing device to offline
                var updated = existing with
                {
                    Status = "Offline",
                    LastHeartbeatAtUtc = @event.LastHeartbeatAtUtc,
                    UpdatedAtUtc = @event.DetectedAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(updated);

                _logger.LogInformation(
                    "Device marked offline in read model. DeviceId={DeviceId}, Code={Code}",
                    @event.DeviceId, @event.DeviceCode);
            }
            else
            {
                // Device doesn't exist in read model yet (may arrive out of order)
                // Create a minimal entry to track offline state
                _logger.LogWarning(
                    "Device not found in read model for offline event. Creating entry. " +
                    "DeviceId={DeviceId}, Code={Code}",
                    @event.DeviceId, @event.DeviceCode);

                var newDevice = new DeviceReadModel
                {
                    DeviceId = @event.DeviceId,
                    DeviceCode = @event.DeviceCode,
                    DeviceName = @event.DeviceCode,
                    DeviceType = "Unknown",
                    Status = "Offline",
                    LinkedAssetId = @event.LinkedAssetId,
                    LastHeartbeatAtUtc = @event.LastHeartbeatAtUtc,
                    RegisteredAtUtc = @event.OccurredAtUtc,
                    UpdatedAtUtc = @event.DetectedAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(newDevice);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error consuming DeviceOfflineV1 event. DeviceId={DeviceId}, Code={Code}",
                @event.DeviceId, @event.DeviceCode);
            throw;
        }
    }
}