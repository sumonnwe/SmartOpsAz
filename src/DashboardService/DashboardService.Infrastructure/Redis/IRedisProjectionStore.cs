using DashboardService.Domain.ReadModels;

namespace DashboardService.Infrastructure.Redis;

/// <summary>
/// Interface for Redis-backed projection storage.
/// Handles read model persistence and retrieval.
/// </summary>
public interface IRedisProjectionStore
{
    // Asset operations
    Task<bool> AssetExistsAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<AssetReadModel?> GetAssetAsync(Guid assetId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetReadModel>> GetAllAssetsAsync(CancellationToken cancellationToken = default);
    Task UpdateAssetAsync(AssetReadModel asset, CancellationToken cancellationToken = default);
    Task<int> CountAssetsByStatusAsync(string status, CancellationToken cancellationToken = default);

    // Device operations
    Task<bool> DeviceExistsAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<DeviceReadModel?> GetDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceReadModel>> GetAllDevicesAsync(CancellationToken cancellationToken = default);
    Task UpdateDeviceAsync(DeviceReadModel device, CancellationToken cancellationToken = default);
    Task<int> CountDevicesByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<int> CountAllDevicesAsync(CancellationToken cancellationToken = default);
    Task<int> CountAllAssetsAsync(CancellationToken cancellationToken = default);
}