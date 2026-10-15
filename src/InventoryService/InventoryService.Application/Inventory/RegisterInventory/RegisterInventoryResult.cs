using System;

namespace InventoryService.Application.Inventory.RegisterInventory;

public sealed class RegisterInventoryResult
{
    public Guid InventoryId { get; init; }
    public string AssetTag { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
}