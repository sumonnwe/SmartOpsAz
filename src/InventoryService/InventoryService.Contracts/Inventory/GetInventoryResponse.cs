using System;

namespace InventoryService.Contracts.Inventory;

public sealed class GetInventoryResponse
{
    public Guid InventoryId { get; set; }
    public string AssetTag { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public string DeviceName { get; set; } = null!;
    public string? DeviceType { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    // use integer to keep Contracts independent of Domain enums
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}  