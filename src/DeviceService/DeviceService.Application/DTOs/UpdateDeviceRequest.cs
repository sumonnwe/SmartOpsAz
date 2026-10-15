namespace DeviceService.Application.DTOs;

public sealed record UpdateDeviceRequest(
    string Name,
    string? FirmwareVersion
);