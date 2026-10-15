using DeviceService.Domain.Enums;
using DeviceService.Domain.Events;
using DeviceService.Domain.Exceptions;

namespace DeviceService.Domain.Entities;

public class IoTDevice
{
    public Guid Id { get; private set; }
    public string DeviceCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DeviceType DeviceType { get; private set; }
    public Guid? GatewayId { get; private set; }
    public Guid? LinkedAssetId { get; private set; }
    public string? FirmwareVersion { get; private set; }
    public DeviceStatus Status { get; private set; }
    public decimal? BatteryPercentage { get; private set; }
    public int? SignalStrengthRssi { get; private set; }
    public decimal? CpuUsagePercentage { get; private set; }
    public decimal? MemoryUsagePercentage { get; private set; }
    public DateTimeOffset? LastHeartbeatAtUtc { get; private set; }
    public DateTimeOffset RegisteredAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public bool HasPublishedOfflineEvent { get; private set; }

    private readonly List<DomainEvent> _events = new();
    public IReadOnlyCollection<DomainEvent> Events => _events.AsReadOnly();

    // EF constructor
    protected IoTDevice() { }

    private IoTDevice(Guid id, string deviceCode, string name, DeviceType deviceType, DateTimeOffset now)
    {
        Id = id;
        DeviceCode = deviceCode;
        Name = name;
        DeviceType = deviceType;
        Status = DeviceStatus.Offline;
        RegisteredAtUtc = now;
        HasPublishedOfflineEvent = false;
    }

    public static IoTDevice RegisterNew(
        string deviceCode,
        string name,
        DeviceType deviceType,
        Guid? gatewayId,
        string? firmwareVersion,
        DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(deviceCode))
            throw new DomainException("Device.InvalidDeviceCode", "DeviceCode is required.");
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 150)
            throw new DomainException("Device.InvalidName", "Name must be between 2 and 150 characters.");
        if (firmwareVersion != null && firmwareVersion.Length > 50)
            throw new DomainException("Device.InvalidFirmwareVersion", "FirmwareVersion must be maximum 50 characters.");

        var device = new IoTDevice(Guid.NewGuid(), deviceCode.Trim(), name.Trim(), deviceType, now);
        device.GatewayId = gatewayId;
        device.FirmwareVersion = firmwareVersion?.Trim();
        device._events.Add(new DeviceRegisteredEvent(device.Id, device.DeviceCode, device.Name));
        return device;
    }

    public void LinkAsset(Guid? assetId, DateTimeOffset now)
    {
        LinkedAssetId = assetId;
        UpdatedAtUtc = now;
    }

    public void ApplyHeartbeat(
        DateTimeOffset recordedAtUtc,
        decimal? battery,
        int? rssi,
        decimal? cpu,
        decimal? memory,
        string? firmware,
        DateTimeOffset serverReceivedAtUtc)
    {
        ValidateHeartbeatMetrics(battery, rssi, cpu, memory);

        var wasOffline = Status == DeviceStatus.Offline;

        BatteryPercentage = battery;
        SignalStrengthRssi = rssi;
        CpuUsagePercentage = cpu;
        MemoryUsagePercentage = memory;
        if (!string.IsNullOrWhiteSpace(firmware))
            FirmwareVersion = firmware!;
        LastHeartbeatAtUtc = serverReceivedAtUtc;
        UpdatedAtUtc = serverReceivedAtUtc;

        // Only set Online if not in Maintenance
        if (Status != DeviceStatus.Maintenance)
        {
            Status = DeviceStatus.Online;
        }

        // Publish recovery event if device was offline
        if (wasOffline && Status == DeviceStatus.Online)
        {
            _events.Add(new DeviceRecoveredEvent(Id, DeviceCode, serverReceivedAtUtc));
            HasPublishedOfflineEvent = false; // Reset flag on recovery
        }
    }

    public void MarkOffline(DateTimeOffset now)
    {
        if (Status == DeviceStatus.Online)
        {
            Status = DeviceStatus.Offline;
            UpdatedAtUtc = now;

            // Only publish offline event if we haven't already
            if (!HasPublishedOfflineEvent)
            {
                _events.Add(new DeviceOfflineEvent(Id, DeviceCode, LastHeartbeatAtUtc ?? now));
                HasPublishedOfflineEvent = true;
            }
        }
    }

    public void SetMaintenance(DateTimeOffset now)
    {
        Status = DeviceStatus.Maintenance;
        UpdatedAtUtc = now;
    }

    public void Activate(DateTimeOffset now)
    {
        if (Status == DeviceStatus.Maintenance)
        {
            Status = DeviceStatus.Online;
            UpdatedAtUtc = now;
        }
    }

    public void UpdateName(string name, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 150)
            throw new DomainException("Device.InvalidName", "Name must be between 2 and 150 characters.");
        Name = name.Trim();
        UpdatedAtUtc = now;
    }

    public void ClearEvents()
    {
        _events.Clear();
    }

    private static void ValidateHeartbeatMetrics(decimal? battery, int? rssi, decimal? cpu, decimal? memory)
    {
        if (battery.HasValue && (battery < 0 || battery > 100))
            throw new DomainException("Device.InvalidBattery", "BatteryPercentage must be between 0 and 100.");
        if (rssi.HasValue && (rssi < -120 || rssi > 0))
            throw new DomainException("Device.InvalidRSSI", "SignalStrengthRssi must be between -120 and 0.");
        if (cpu.HasValue && (cpu < 0 || cpu > 100))
            throw new DomainException("Device.InvalidCPU", "CpuUsagePercentage must be between 0 and 100.");
        if (memory.HasValue && (memory < 0 || memory > 100))
            throw new DomainException("Device.InvalidMemory", "MemoryUsagePercentage must be between 0 and 100.");
    }
}