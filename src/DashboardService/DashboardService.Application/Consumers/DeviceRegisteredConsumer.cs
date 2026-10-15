using DashboardService.Domain.ReadModels;
using DashboardService.Infrastructure.Redis;
using MassTransit;
using Microsoft.Extensions.Logging;
using SmartOpsAz.IntegrationEvents.Events;

namespace DashboardService.Application.Consumers;

/// <summary>
/// Consumes DeviceRegisteredV1 events and creates new device read models.
/// Implements idempotent processing to handle duplicate events.
/// </summary>
public sealed class DeviceRegisteredConsumer : IConsumer<DeviceRegisteredV1>
{
    private readonly IRedisProjectionStore _projectionStore;
    private readonly ILogger<DeviceRegisteredConsumer> _logger;

    public DeviceRegisteredConsumer(
        IRedisProjectionStore projectionStore,
        ILogger<DeviceRegisteredConsumer> logger)
    {
        _projectionStore = projectionStore ?? throw new ArgumentNullException(nameof(projectionStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeviceRegisteredV1> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogInformation(
                "Consuming DeviceRegisteredV1 event. DeviceId={DeviceId}, Code={Code}, Name={Name}, CorrelationId={CorrelationId}",
                @event.DeviceId, @event.DeviceCode, @event.DeviceName, @event.CorrelationId);

            // Idempotency check: if device already exists, don't recreate it
            var existing = await _projectionStore.GetDeviceAsync(@event.DeviceId);
            if (existing != null)
            {
                _logger.LogInformation(
                    "Device already exists in read model. Skipping registration. DeviceId={DeviceId}",
                    @event.DeviceId);
                return;
            }

            // Create new device read model
            var device = new DeviceReadModel
            {
                DeviceId = @event.DeviceId,
                DeviceCode = @event.DeviceCode,
                DeviceName = @event.DeviceName,
                DeviceType = @event.DeviceType,
                Status = "Offline", // New devices start as offline until first heartbeat
                LinkedAssetId = @event.LinkedAssetId,
                FirmwareVersion = @event.FirmwareVersion,
                RegisteredAtUtc = @event.RegisteredAtUtc,
                EventVersion = 1
            };

            await _projectionStore.UpdateDeviceAsync(device);

            _logger.LogInformation(
                "Device registered in read model. DeviceId={DeviceId}, Code={Code}, Name={Name}",
                @event.DeviceId, @event.DeviceCode, @event.DeviceName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error consuming DeviceRegisteredV1 event. DeviceId={DeviceId}, Code={Code}",
                @event.DeviceId, @event.DeviceCode);
            throw;
        }
    }
}