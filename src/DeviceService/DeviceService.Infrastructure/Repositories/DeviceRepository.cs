using DeviceService.Application.Interfaces;
using DeviceService.Domain.Entities;
using DeviceService.Domain.Enums;
using DeviceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DeviceService.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly DeviceDbContext _db;

    public DeviceRepository(DeviceDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(IoTDevice device, CancellationToken cancellationToken = default)
    {
        await _db.Devices.AddAsync(device, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IoTDevice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Devices.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<IoTDevice?> GetByDeviceCodeAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        return await _db.Devices.FirstOrDefaultAsync(d => d.DeviceCode == deviceCode, cancellationToken);
    }

    public async Task UpdateAsync(IoTDevice device, CancellationToken cancellationToken = default)
    {
        _db.Devices.Update(device);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddHeartbeatAsync(DeviceHeartbeat heartbeat, CancellationToken cancellationToken = default)
    {
        await _db.DeviceHeartbeats.AddAsync(heartbeat, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DeviceHeartbeat>> GetHeartbeatsAsync(
        Guid deviceId,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return await _db.DeviceHeartbeats
            .Where(h => h.DeviceId == deviceId)
            .OrderByDescending(h => h.RecordedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IoTDevice>> QueryAsync(
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await _db.Devices
            .OrderByDescending(d => d.RegisteredAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IoTDevice>> GetDevicesWithLastHeartbeatBeforeAsync(
        DateTimeOffset thresholdUtc,
        CancellationToken cancellationToken = default)
    {
        return await _db.Devices
            .Where(d => d.LastHeartbeatAtUtc != null 
                && d.LastHeartbeatAtUtc < thresholdUtc 
                && d.Status == DeviceStatus.Online)
            .ToListAsync(cancellationToken);
    }
}