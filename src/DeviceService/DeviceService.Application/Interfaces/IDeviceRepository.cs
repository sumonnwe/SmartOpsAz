using DeviceService.Domain.Entities;

namespace DeviceService.Application.Interfaces;

public interface IDeviceRepository
{
    Task AddAsync(IoTDevice device, CancellationToken cancellationToken = default);
    Task<IoTDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IoTDevice?> GetByDeviceCodeAsync(string deviceCode, CancellationToken cancellationToken = default);
    Task UpdateAsync(IoTDevice device, CancellationToken cancellationToken = default);
    Task AddHeartbeatAsync(DeviceHeartbeat heartbeat, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeviceHeartbeat>> GetHeartbeatsAsync(Guid deviceId, int page = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IoTDevice>> QueryAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IoTDevice>> GetDevicesWithLastHeartbeatBeforeAsync(DateTimeOffset thresholdUtc, CancellationToken cancellationToken = default);
}