//csharp src/InventoryService/InventoryService.Contracts/Inventory/RegisterInventoryRequest.cs
namespace InventoryService.Contracts.Inventory;

public sealed class RegisterInventoryRequest
{
    public string AssetTag { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
    public string DeviceName { get; init; } = null!;
    public string? DeviceType { get; init; }
}