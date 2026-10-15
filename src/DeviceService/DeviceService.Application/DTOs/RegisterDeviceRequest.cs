using DeviceService.Domain.Enums;

namespace DeviceService.Application.DTOs;

public sealed record RegisterDeviceRequest(
    string DeviceCode,
    string Name,
    DeviceType DeviceType,
    Guid? GatewayId,
    string? FirmwareVersion
);