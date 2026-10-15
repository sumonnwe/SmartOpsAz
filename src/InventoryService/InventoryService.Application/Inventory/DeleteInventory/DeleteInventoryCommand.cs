using System;
using MediatR;

namespace InventoryService.Application.Inventory.DeleteInventory;

public sealed class DeleteInventoryCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
