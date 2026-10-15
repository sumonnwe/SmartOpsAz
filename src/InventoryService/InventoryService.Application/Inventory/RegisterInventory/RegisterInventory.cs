//csharp src/InventoryService/InventoryService.Application/Inventory/RegisterInventory/RegisterInventoryCommand.cs
using MediatR;
using System;

namespace InventoryService.Application.Inventory.RegisterInventory;

public sealed class RegisterInventory: IRequest<RegisterInventoryResult>
{
    public string AssetTag { get; init; } = null!;
    public string SerialNumber { get; init; } = null!;
    public string DeviceName { get; init; } = null!;
    public string? DeviceType { get; init; }
}