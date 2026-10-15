using DeviceService.Domain.Enums;

namespace DeviceService.Application.DTOs;

public sealed record DeviceDto(
    Guid Id,
    string DeviceCode,
    string Name,
    DeviceType DeviceType,
    Guid? GatewayId,
    Guid? LinkedAssetId,
    string? FirmwareVersion,
    DeviceStatus Status,
    decimal? BatteryPercentage,
    int? SignalStrengthRssi,
    decimal? CpuUsagePercentage,
    decimal? MemoryUsagePercentage,
    DateTimeOffset? LastHeartbeatAtUtc,
    DateTimeOffset RegisteredAtUtc,
    DateTimeOffset? UpdatedAtUtc
);