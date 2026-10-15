using System;
using MediatR;

namespace InventoryService.Application.Inventory.RetireInventory;

public sealed class RetireInventoryCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
