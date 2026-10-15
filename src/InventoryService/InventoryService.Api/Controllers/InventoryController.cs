using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.AssignDepartment;
using InventoryService.Application.Inventory.DeleteInventory;
using InventoryService.Application.Inventory.GetAssetHistory;
using InventoryService.Application.Inventory.GetInventoryById;
using InventoryService.Application.Inventory.RegisterInventory;
using InventoryService.Application.Inventory.RetireInventory;
using InventoryService.Application.Inventory.SearchInventory;
using InventoryService.Application.Inventory.TransferLocation;
using InventoryService.Application.Inventory.UpdateInventory;
using InventoryService.Contracts.Inventory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InventoryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InventoryController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(ISender sender, ILogger<InventoryController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Register a new inventory item.
    /// </summary>
    /// <response code="201">Created with Inventory id</response>
    /// <response code="400">Invalid request</response>
    /// <response code="409">Business conflict (duplicate asset tag or serial number)</response>
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterInventoryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /api/inventory Register request for AssetTag {AssetTag}", request.AssetTag);

        var command = new RegisterInventory
        {
            AssetTag = request.AssetTag,
            SerialNumber = request.SerialNumber,
            DeviceName = request.DeviceName,
            DeviceType = request.DeviceType
        };

        var result = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.InventoryId }, result);
    }

    /// <summary>
    /// Search and filter inventory items with pagination.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? search,
        [FromQuery] int? status,
        [FromQuery] string? department,
        [FromQuery] string? location,
        [FromQuery] string? deviceType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GET /api/inventory Search with filters");

        var query = new SearchInventoryQuery
        {
            Search = search,
            Status = status,
            Department = department,
            Location = location,
            DeviceType = deviceType,
            Page = page,
            PageSize = pageSize
        };

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get inventory by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInventoryByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Update editable inventory metadata.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateInventoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateInventoryCommand
        {
            Id = id,
            DeviceName = request.DeviceName,
            DeviceType = request.DeviceType
        };

        var updated = await _sender.Send(command, cancellationToken);
        if (!updated) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Soft delete an inventory item.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("DELETE /api/inventory/{id} Delete request", id);

        var command = new DeleteInventoryCommand { Id = id };
        var deleted = await _sender.Send(command, cancellationToken);

        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Assign inventory to a department.
    /// </summary>
    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> AssignDepartment([FromRoute] Guid id, [FromBody] AssignDepartmentRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /api/inventory/{id}/assign request", id);

        var command = new AssignDepartmentCommand
        {
            Id = id,
            Department = request.Department
        };

        var assigned = await _sender.Send(command, cancellationToken);

        if (!assigned) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Transfer inventory to a new location.
    /// </summary>
    [HttpPost("{id:guid}/transfer")]
    public async Task<IActionResult> TransferLocation([FromRoute] Guid id, [FromBody] TransferLocationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /api/inventory/{id}/transfer request", id);

        var command = new TransferLocationCommand
        {
            Id = id,
            Location = request.Location
        };

        var transferred = await _sender.Send(command, cancellationToken);

        if (!transferred) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Retire an inventory item (Biomedical Engineer only).
    /// </summary>
    [Authorize(Policy = "BiomedicalEngineerOnly")]
    [HttpPost("{id:guid}/retire")]
    public async Task<IActionResult> Retire([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("POST /api/inventory/{id}/retire request", id);

        var command = new RetireInventoryCommand { Id = id };
        var retired = await _sender.Send(command, cancellationToken);

        if (!retired) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get asset movement history.
    /// </summary>
    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> GetHistory([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET /api/inventory/{id}/history request", id);

        var query = new GetAssetHistoryQuery { InventoryId = id };
        var result = await _sender.Send(query, cancellationToken);

        return Ok(result);
    }
}