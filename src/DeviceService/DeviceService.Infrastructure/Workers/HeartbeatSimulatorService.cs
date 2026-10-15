using DeviceService.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DeviceService.Infrastructure.Workers;

public class HeartbeatSimulatorService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HeartbeatSimulatorService> _logger;
    private readonly IConfiguration _config;

    public HeartbeatSimulatorService(
        IHttpClientFactory httpClientFactory,
        ILogger<HeartbeatSimulatorService> logger,
        IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _config.GetValue<bool>("DeviceService:Simulator:Enabled");
        if (!enabled)
        {
            _logger.LogInformation("Heartbeat simulator is disabled.");
            return;
        }

        var intervalSeconds = _config.GetValue<int?>("DeviceService:Simulator:IntervalSeconds") ?? 60;
        var deviceIdStr = _config.GetValue<string>("DeviceService:Simulator:DeviceId");

        if (string.IsNullOrEmpty(deviceIdStr) || !Guid.TryParse(deviceIdStr, out var deviceId))
        {
            _logger.LogWarning("Simulator enabled but DeviceId is not configured or invalid.");
            return;
        }

        _logger.LogInformation(
            "Heartbeat simulator started for device {DeviceId}. Interval={Interval}s",
            deviceId, intervalSeconds);

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SendHeartbeatAsync(deviceId, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("HeartbeatSimulatorService is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in HeartbeatSimulatorService.");
        }
    }

    private async Task SendHeartbeatAsync(Guid deviceId, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("DeviceServiceSimulator");
            var now = DateTimeOffset.UtcNow;
            var rand = Random.Shared;

            var hb = new HeartbeatRequest(
                RecordedAtUtc: now,
                BatteryPercentage: (decimal)(rand.NextDouble() * 40 + 50),
                SignalStrengthRssi: -50 - rand.Next(0, 70),
                CpuUsagePercentage: (decimal)(rand.NextDouble() * 30),
                MemoryUsagePercentage: (decimal)(rand.NextDouble() * 50),
                FirmwareVersion: "sim-1.0.0"
            );

            var response = await client.PostAsJsonAsync(
                $"/api/v1/devices/{deviceId}/heartbeat",
                hb,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug("Heartbeat sent for device {DeviceId}: {Status}", deviceId, response.StatusCode);
            }
            else
            {
                _logger.LogWarning("Heartbeat failed for device {DeviceId}: {Status}", deviceId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending simulated heartbeat for device {DeviceId}.", deviceId);
        }
    }
}