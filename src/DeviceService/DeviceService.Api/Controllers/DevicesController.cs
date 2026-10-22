using DeviceService.Application.DTOs;
using DeviceService.Application.Interfaces;
using DeviceService.Domain.Entities;
using DeviceService.Domain.Enums;
using DeviceService.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace DeviceService.Api.Controllers;

[ApiController]
[Route("api/v1/devices")]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDeviceRepository _repo;
    private readonly IInventoryServiceClient _inventoryClient;
    private readonly IEventPublisher _publisher;
    private readonly IValidator<RegisterDeviceRequest> _registerValidator;
    private readonly IValidator<HeartbeatRequest> _heartbeatValidator;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(
        IDeviceRepository repo,
        IInventoryServiceClient inventoryClient,
        IEventPublisher publisher,
        IValidator<RegisterDeviceRequest> registerValidator,
        IValidator<HeartbeatRequest> heartbeatValidator,
        ILogger<DevicesController> logger)
    {
        _repo = repo;
        _inventoryClient = inventoryClient;
        _publisher = publisher;
        _registerValidator = registerValidator;
        _heartbeatValidator = heartbeatValidator;
        _logger = logger;
    }

    /// <summary>
    /// Register a new IoT device.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "BiomedicalEngineer,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var existing = await _repo.GetByDeviceCodeAsync(request.DeviceCode, cancellationToken);
        if (existing != null)
            return Conflict(new { message = "Device with this DeviceCode already exists." });

        try
        {
            var now = DateTimeOffset.UtcNow;
            var device = IoTDevice.RegisterNew(
                request.DeviceCode,
                request.Name,
                request.DeviceType,
                request.GatewayId,
                request.FirmwareVersion,
                now);

            await _repo.AddAsync(device, cancellationToken);
            // Publish device registered event
            await _publisher.PublishDeviceRegisteredAsync(
                device.Id,
                device.DeviceCode,
                device.Name,
                device.DeviceType.ToString(),
                device.LinkedAssetId,
                device.FirmwareVersion,
                now,
                Guid.NewGuid(),
                cancellationToken);

            var dto = MapToDto(device);
            _logger.LogInformation("Device registered: {DeviceId} ({Code})", device.Id, device.DeviceCode);

            return CreatedAtAction(nameof(GetDevice), new { deviceId = device.Id }, dto);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Domain exception during device registration: {Code} - {Message}", ex.Code, ex.Message);
            return BadRequest(new { error = ex.Code, message = ex.Message });
        }
    }

    /// <summary>
    /// Get a device by ID.
    /// </summary>
    [HttpGet("{deviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDevice(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        var dto = MapToDto(device);
        return Ok(dto);
    }

    /// <summary>
    /// Query devices with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryDevices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var devices = await _repo.QueryAsync(page, pageSize, cancellationToken);
        var dtos = devices.Select(MapToDto).ToList();

        return Ok(new { page, pageSize, items = dtos });
    }

    /// <summary>
    /// Update device details.
    /// </summary>
    [HttpPut("{deviceId:guid}")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDevice(
        [FromRoute] Guid deviceId,
        [FromBody] UpdateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        try
        {
            var now = DateTimeOffset.UtcNow;
            device.UpdateName(request.Name, now);

            if (!string.IsNullOrWhiteSpace(request.FirmwareVersion))
            {
                device.ApplyHeartbeat(
                    device.LastHeartbeatAtUtc ?? now,
                    device.BatteryPercentage,
                    device.SignalStrengthRssi,
                    device.CpuUsagePercentage,
                    device.MemoryUsagePercentage,
                    request.FirmwareVersion,
                    now);
            }

            await _repo.UpdateAsync(device, cancellationToken);
            _logger.LogInformation("Device updated: {DeviceId}", deviceId);

            return NoContent();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Domain exception during device update: {Code} - {Message}", ex.Code, ex.Message);
            return BadRequest(new { error = ex.Code, message = ex.Message });
        }
    }

    /// <summary>
    /// Link a device to an inventory asset.
    /// </summary>
    [HttpPost("{deviceId:guid}/link-asset")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> LinkAsset(
        [FromRoute] Guid deviceId,
        [FromBody] LinkAssetRequest request,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound(new { message = "Device not found." });

        // Validate asset exists in InventoryService
        var assetExists = await _inventoryClient.AssetExistsAndIsActiveAsync(request.AssetId, cancellationToken);
        if (!assetExists)
            return Conflict(new { message = "Asset not found or is not active in InventoryService." });

        device.LinkAsset(request.AssetId, DateTimeOffset.UtcNow);
        await _repo.UpdateAsync(device, cancellationToken);

        _logger.LogInformation("Asset {AssetId} linked to device {DeviceId}", request.AssetId, deviceId);
        return NoContent();
    }

    /// <summary>
    /// Unlink a device from its inventory asset.
    /// </summary>
    [HttpDelete("{deviceId:guid}/link-asset")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnlinkAsset(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        device.LinkAsset(null, DateTimeOffset.UtcNow);
        await _repo.UpdateAsync(device, cancellationToken);

        _logger.LogInformation("Asset unlinked from device {DeviceId}", deviceId);
        return NoContent();
    }

    /// <summary>
    /// Submit a heartbeat for a device.
    /// </summary>
    [HttpPost("{deviceId:guid}/heartbeat")]
    [AllowAnonymous] // In production, use API key or specific claim
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitHeartbeat(
        [FromRoute] Guid deviceId,
        [FromBody] HeartbeatRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _heartbeatValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        try
        {
            var serverReceivedAt = DateTimeOffset.UtcNow;
            var wasOffline = device.Status == DeviceStatus.Offline;

            device.ApplyHeartbeat(
                request.RecordedAtUtc,
                request.BatteryPercentage,
                request.SignalStrengthRssi,
                request.CpuUsagePercentage,
                request.MemoryUsagePercentage,
                request.FirmwareVersion,
                serverReceivedAt);

            await _repo.UpdateAsync(device, cancellationToken);

            // Persist heartbeat
            var heartbeat = new DeviceHeartbeat(
                deviceId,
                request.RecordedAtUtc,
                request.BatteryPercentage,
                request.SignalStrengthRssi,
                request.CpuUsagePercentage,
                request.MemoryUsagePercentage,
                request.FirmwareVersion,
                serverReceivedAt);

            await _repo.AddHeartbeatAsync(heartbeat, cancellationToken);

            // Publish recovery event if device recovered from offline
            if (wasOffline && device.Status == DeviceStatus.Online)
            {
                await _publisher.PublishDeviceRecoveredAsync(
                    device.Id,
                    device.DeviceCode,
                    device.LinkedAssetId,
                    serverReceivedAt,
                    Guid.NewGuid(),
                    cancellationToken);
            }

            _logger.LogDebug("Heartbeat received for device {DeviceId}", deviceId);
            return Accepted();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Domain exception during heartbeat: {Code} - {Message}", ex.Code, ex.Message);
            return BadRequest(new { error = ex.Code, message = ex.Message });
        }
    }

    /// <summary>
    /// Get heartbeat history for a device.
    /// </summary>
    [HttpGet("{deviceId:guid}/heartbeats")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHeartbeats(
        [FromRoute] Guid deviceId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 50;

        var heartbeats = await _repo.GetHeartbeatsAsync(deviceId, page, pageSize, cancellationToken);
        var dtos = heartbeats.Select(h => new HeartbeatDto(
            h.Id,
            h.DeviceId,
            h.RecordedAtUtc,
            h.BatteryPercentage,
            h.SignalStrengthRssi,
            h.CpuUsagePercentage,
            h.MemoryUsagePercentage,
            h.FirmwareVersion,
            h.ReceivedAtUtc)).ToList();

        return Ok(new { page, pageSize, items = dtos });
    }

    /// <summary>
    /// Mark a device for maintenance.
    /// </summary>
    [HttpPost("{deviceId:guid}/maintenance")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMaintenance(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        device.SetMaintenance(DateTimeOffset.UtcNow);
        await _repo.UpdateAsync(device, cancellationToken);

        _logger.LogInformation("Device set to maintenance: {DeviceId}", deviceId);
        return NoContent();
    }

    /// <summary>
    /// Activate a device (typically from maintenance).
    /// </summary>
    [HttpPost("{deviceId:guid}/activate")]
    [Authorize(Roles = "BiomedicalEngineer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        device.Activate(DateTimeOffset.UtcNow);
        await _repo.UpdateAsync(device, cancellationToken);

        _logger.LogInformation("Device activated: {DeviceId}", deviceId);
        return NoContent();
    }

    /// <summary>
    /// Delete a device.
    /// </summary>
    [HttpDelete("{deviceId:guid}")]
    [Authorize(Roles = "BiomedicalEngineer,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDevice(
        [FromRoute] Guid deviceId,
        CancellationToken cancellationToken)
    {
        var device = await _repo.GetByIdAsync(deviceId, cancellationToken);
        if (device == null)
            return NotFound();

        await _repo.DeleteAsync(deviceId, cancellationToken);
        _logger.LogInformation("Device deleted: {DeviceId}", deviceId);
        return NoContent();
    }

    /// <summary>
    /// Search devices with filtering and pagination.
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchDevices(
        [FromQuery] string? deviceCode,
        [FromQuery] DeviceStatus? status,
        [FromQuery] Guid? gatewayId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var devices = await _repo.SearchAsync(deviceCode, status, gatewayId, page, pageSize, cancellationToken);
        var dtos = devices.Select(MapToDto).ToList();

        return Ok(new { page, pageSize, items = dtos });
    }

    private static DeviceDto MapToDto(IoTDevice device)
    {
        return new DeviceDto(
            device.Id,
            device.DeviceCode,
            device.Name,
            device.DeviceType,
            device.GatewayId,
            device.LinkedAssetId,
            device.FirmwareVersion,
            device.Status,
            device.BatteryPercentage,
            device.SignalStrengthRssi,
            device.CpuUsagePercentage,
            device.MemoryUsagePercentage,
            device.LastHeartbeatAtUtc,
            device.RegisteredAtUtc,
            device.UpdatedAtUtc);
    }
}