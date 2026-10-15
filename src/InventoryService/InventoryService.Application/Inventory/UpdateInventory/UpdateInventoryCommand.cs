//csharp src/InventoryService/InventoryService.Application/Inventory/UpdateInventory/UpdateInventoryCommand.cs
using MediatR;
using System;

namespace InventoryService.Application.Inventory.UpdateInventory;

public sealed class UpdateInventoryCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string DeviceName { get; init; } = null!;
    public string? DeviceType { get; init; }
}