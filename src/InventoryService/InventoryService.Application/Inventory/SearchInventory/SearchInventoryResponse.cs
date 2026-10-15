using System;
using System.Collections.Generic;

namespace InventoryService.Application.Inventory.SearchInventory;

public sealed class SearchInventoryResponse
{
    public List<InventoryItemDto> Items { get; init; } = new();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}

public sealed class InventoryItemDto
{
    public Guid InventoryId { get; init; }
    public string AssetTag { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
    public string DeviceName { get; init; } = null!;
    public string? DeviceType { get; init; }
    public string? Department { get; init; }
    public string? Location { get; init; }
    public int Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}