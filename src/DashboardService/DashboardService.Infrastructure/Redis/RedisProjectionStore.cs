using DashboardService.Domain.ReadModels;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace DashboardService.Infrastructure.Redis;

/// <summary>
/// Redis-backed read model projection store.
/// Handles idempotent event-driven projection updates.
/// Uses JSON serialization for storage and retrieval.
/// </summary>
public sealed class RedisProjectionStore : IRedisProjectionStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisProjectionStore> _logger;

    private const string AssetKeyPrefix = "asset:";
    private const string DeviceKeyPrefix = "device:";
    private const string AssetCountKeyPrefix = "asset-count:";
    private const string DeviceCountKeyPrefix = "device-count:";

    public RedisProjectionStore(
        IConnectionMultiplexer redis,
        ILogger<RedisProjectionStore> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ============= Asset Operations =============

    public async Task<bool> AssetExistsAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"{AssetKeyPrefix}{assetId}");
    }

    public async Task<AssetReadModel?> GetAssetAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync($"{AssetKeyPrefix}{assetId}");

        if (!json.HasValue)
            return null;

        try
        {
            return JsonSerializer.Deserialize<AssetReadModel>(json.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize asset {AssetId}", assetId);
            return null;
        }
    }

    public async Task<IReadOnlyList<AssetReadModel>> GetAllAssetsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var db = _redis.GetDatabase();
            var keys = server.Keys(pattern: $"{AssetKeyPrefix}*");

            var assets = new List<AssetReadModel>();
            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (json.HasValue)
                {
                    try
                    {
                        var asset = JsonSerializer.Deserialize<AssetReadModel>(json.ToString());
                        if (asset != null)
                            assets.Add(asset);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize asset from key {Key}", key);
                    }
                }
            }

            return assets.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all assets");
            return new List<AssetReadModel>().AsReadOnly();
        }
    }

    public async Task UpdateAssetAsync(AssetReadModel asset, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(asset);

            await db.StringSetAsync(
                $"{AssetKeyPrefix}{asset.AssetId}",
                json,
                expiry: TimeSpan.FromHours(24));

            _logger.LogDebug("Asset {AssetId} updated in Redis", asset.AssetId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update asset {AssetId}", asset.AssetId);
            throw;
        }
    }

    public async Task<int> CountAssetsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        try
        {
            var assets = await GetAllAssetsAsync(cancellationToken);
            return assets.Count(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count assets by status {Status}", status);
            return 0;
        }
    }

    // ============= Device Operations =============

    public async Task<bool> DeviceExistsAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync($"{DeviceKeyPrefix}{deviceId}");
    }

    public async Task<DeviceReadModel?> GetDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync($"{DeviceKeyPrefix}{deviceId}");

        if (!json.HasValue)
            return null;

        try
        {
            return JsonSerializer.Deserialize<DeviceReadModel>(json.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize device {DeviceId}", deviceId);
            return null;
        }
    }

    public async Task<IReadOnlyList<DeviceReadModel>> GetAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var db = _redis.GetDatabase();
            var keys = server.Keys(pattern: $"{DeviceKeyPrefix}*");

            var devices = new List<DeviceReadModel>();
            foreach (var key in keys)
            {
                var json = await db.StringGetAsync(key);
                if (json.HasValue)
                {
                    try
                    {
                        var device = JsonSerializer.Deserialize<DeviceReadModel>(json.ToString());
                        if (device != null)
                            devices.Add(device);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize device from key {Key}", key);
                    }
                }
            }

            return devices.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all devices");
            return new List<DeviceReadModel>().AsReadOnly();
        }
    }

    public async Task UpdateDeviceAsync(DeviceReadModel device, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(device);

            await db.StringSetAsync(
                $"{DeviceKeyPrefix}{device.DeviceId}",
                json,
                expiry: TimeSpan.FromHours(24));

            _logger.LogDebug("Device {DeviceId} updated in Redis", device.DeviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update device {DeviceId}", device.DeviceId);
            throw;
        }
    }

    public async Task<int> CountDevicesByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        try
        {
            var devices = await GetAllDevicesAsync(cancellationToken);
            return devices.Count(d => d.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count devices by status {Status}", status);
            return 0;
        }
    }

    public async Task<int> CountAllDevicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var devices = await GetAllDevicesAsync(cancellationToken);
            return devices.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count all devices");
            return 0;
        }
    }

    public async Task<int> CountAllAssetsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var assets = await GetAllAssetsAsync(cancellationToken);
            return assets.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to count all assets");
            return 0;
        }
    }
}