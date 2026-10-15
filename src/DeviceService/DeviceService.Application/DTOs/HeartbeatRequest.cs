namespace DeviceService.Application.DTOs;

public sealed record HeartbeatRequest(
    DateTimeOffset RecordedAtUtc,
    decimal? BatteryPercentage,
    int? SignalStrengthRssi,
    decimal? CpuUsagePercentage,
    decimal? MemoryUsagePercentage,
    string? FirmwareVersion
);