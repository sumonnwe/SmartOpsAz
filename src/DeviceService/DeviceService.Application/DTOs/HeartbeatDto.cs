namespace DeviceService.Application.DTOs;

public sealed record HeartbeatDto(
    Guid Id,
    Guid DeviceId,
    DateTimeOffset RecordedAtUtc,
    decimal? BatteryPercentage,
    int? SignalStrengthRssi,
    decimal? CpuUsagePercentage,
    decimal? MemoryUsagePercentage,
    string? FirmwareVersion,
    DateTimeOffset ReceivedAtUtc
);