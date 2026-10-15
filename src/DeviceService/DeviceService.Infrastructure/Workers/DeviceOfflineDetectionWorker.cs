using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DeviceService.Application.Interfaces;

namespace DeviceService.Infrastructure.Workers;

public class DeviceOfflineDetectionWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<DeviceOfflineDetectionWorker> _logger;
    private readonly TimeSpan _scanInterval;
    private readonly int _offlineThresholdSeconds;

    public DeviceOfflineDetectionWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DeviceOfflineDetectionWorker> logger,
        IConfiguration config)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _scanInterval = TimeSpan.FromSeconds(config.GetValue<int?>("DeviceService:OfflineDetectionIntervalSeconds") ?? 60);
        _offlineThresholdSeconds = config.GetValue<int?>("DeviceService:OfflineThresholdSeconds") ?? 300;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "DeviceOfflineDetectionWorker started. ScanInterval={Interval}s, OfflineThreshold={Threshold}s",
            _scanInterval.TotalSeconds, _offlineThresholdSeconds);

        using var timer = new PeriodicTimer(_scanInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ScanAndMarkOfflineDevicesAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("DeviceOfflineDetectionWorker is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in DeviceOfflineDetectionWorker.");
            throw;
        }
    }

    private async Task ScanAndMarkOfflineDevicesAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Create a new scope for each execution to resolve scoped dependencies
            using var scope = _serviceScopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var now = DateTimeOffset.UtcNow;
            var thresholdTime = now.AddSeconds(-_offlineThresholdSeconds);

            var offlineDevices = await repo.GetDevicesWithLastHeartbeatBeforeAsync(thresholdTime, cancellationToken);

            foreach (var device in offlineDevices)
            {
                _logger.LogInformation(
                    "Marking device {DeviceId} ({Code}) as offline. LastHB={LastHB}",
                    device.Id, device.DeviceCode, device.LastHeartbeatAtUtc);

                device.MarkOffline(now);
                await repo.UpdateAsync(device, cancellationToken);

                // Publish offline event
                await publisher.PublishDeviceOfflineAsync(
                    device.Id,
                    device.DeviceCode,
                    device.LinkedAssetId,
                    device.LastHeartbeatAtUtc ?? now,
                    _offlineThresholdSeconds,
                    Guid.NewGuid(),
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning devices for offline detection.");
        }
    }
}