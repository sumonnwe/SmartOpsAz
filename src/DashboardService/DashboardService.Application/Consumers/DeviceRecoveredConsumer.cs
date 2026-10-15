using DashboardService.Domain.ReadModels;
using DashboardService.Infrastructure.Redis;
using MassTransit;
using Microsoft.Extensions.Logging;
using SmartOpsAz.IntegrationEvents.Events;

namespace DashboardService.Application.Consumers;

/// <summary>
/// Consumes DeviceRecoveredV1 events and updates the device read model status to Online.
/// Implements idempotent processing to handle duplicate or out-of-order events.
/// </summary>
public sealed class DeviceRecoveredConsumer : IConsumer<DeviceRecoveredV1>
{
    private readonly IRedisProjectionStore _projectionStore;
    private readonly ILogger<DeviceRecoveredConsumer> _logger;

    public DeviceRecoveredConsumer(
        IRedisProjectionStore projectionStore,
        ILogger<DeviceRecoveredConsumer> logger)
    {
        _projectionStore = projectionStore ?? throw new ArgumentNullException(nameof(projectionStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeviceRecoveredV1> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogInformation(
                "Consuming DeviceRecoveredV1 event. DeviceId={DeviceId}, Code={Code}, EventVersion={Version}, CorrelationId={CorrelationId}",
                @event.DeviceId, @event.DeviceCode, @event.EventVersion, @event.CorrelationId);

            // Check if device exists in read model
            var existing = await _projectionStore.GetDeviceAsync(@event.DeviceId);

            if (existing != null)
            {
                // Idempotency check: skip if we've already processed this event or a newer one
                if (existing.EventVersion >= @event.EventVersion)
                {
                    _logger.LogInformation(
                        "Skipping out-of-order or duplicate DeviceRecoveredV1 event. " +
                        "DeviceId={DeviceId}, CurrentVersion={CurrentVersion}, EventVersion={EventVersion}",
                        @event.DeviceId, existing.EventVersion, @event.EventVersion);
                    return;
                }

                // Update existing device to online
                var updated = existing with
                {
                    Status = "Online",
                    UpdatedAtUtc = @event.RecoveredAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(updated);

                _logger.LogInformation(
                    "Device marked online in read model. DeviceId={DeviceId}, Code={Code}",
                    @event.DeviceId, @event.DeviceCode);
            }
            else
            {
                // Device doesn't exist in read model yet
                _logger.LogWarning(
                    "Device not found in read model for recovery event. Creating entry. " +
                    "DeviceId={DeviceId}, Code={Code}",
                    @event.DeviceId, @event.DeviceCode);

                var newDevice = new DeviceReadModel
                {
                    DeviceId = @event.DeviceId,
                    DeviceCode = @event.DeviceCode,
                    DeviceName = @event.DeviceCode,
                    DeviceType = "Unknown",
                    Status = "Online",
                    LinkedAssetId = @event.LinkedAssetId,
                    RegisteredAtUtc = @event.OccurredAtUtc,
                    UpdatedAtUtc = @event.RecoveredAtUtc,
                    EventVersion = @event.EventVersion
                };

                await _projectionStore.UpdateDeviceAsync(newDevice);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error consuming DeviceRecoveredV1 event. DeviceId={DeviceId}, Code={Code}",
                @event.DeviceId, @event.DeviceCode);
            throw;
        }
    }
}