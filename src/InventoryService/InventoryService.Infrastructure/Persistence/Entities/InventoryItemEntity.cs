using System;

namespace InventoryService.Infrastructure.Persistence.Entities;

public sealed class InventoryItemEntity
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; }
    public string SerialNumber { get; set; }
    public string DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}