namespace DeviceService.Domain.Entities;

public class DeviceHeartbeat
{
    public Guid Id { get; private set; }
    public Guid DeviceId { get; private set; }
    public DateTimeOffset RecordedAtUtc { get; private set; }
    public decimal? BatteryPercentage { get; private set; }
    public int? SignalStrengthRssi { get; private set; }
    public decimal? CpuUsagePercentage { get; private set; }
    public decimal? MemoryUsagePercentage { get; private set; }
    public string? FirmwareVersion { get; private set; }
    public DateTimeOffset ReceivedAtUtc { get; private set; }

    // EF constructor
    protected DeviceHeartbeat() { }

    public DeviceHeartbeat(
        Guid deviceId,
        DateTimeOffset recordedAtUtc,
        decimal? battery,
        int? rssi,
        decimal? cpu,
        decimal? memory,
        string? firmware,
        DateTimeOffset receivedAtUtc)
    {
        Id = Guid.NewGuid();
        DeviceId = deviceId;
        RecordedAtUtc = recordedAtUtc;
        BatteryPercentage = battery;
        SignalStrengthRssi = rssi;
        CpuUsagePercentage = cpu;
        MemoryUsagePercentage = memory;
        FirmwareVersion = firmware;
        ReceivedAtUtc = receivedAtUtc;
    }
}