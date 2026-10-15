//csharp src/InventoryService/InventoryService.Contracts/Inventory/UpdateInventoryRequest.cs
namespace InventoryService.Contracts.Inventory;

public sealed class UpdateInventoryRequest
{
    public string DeviceName { get; init; } = null!;
    public string? DeviceType { get; init; }
}