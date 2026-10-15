//csharp src/InventoryService/InventoryService.Contracts/Inventory/RegisterInventoryResponse.cs
using System;

namespace InventoryService.Contracts.Inventory;

public sealed class RegisterInventoryResponse
{
    public Guid InventoryId { get; init; }
    public string AssetTag { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
}