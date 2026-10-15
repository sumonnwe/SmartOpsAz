using System;
using MediatR;

namespace InventoryService.Application.Inventory.TransferLocation;

public sealed class TransferLocationCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Location { get; init; } = null!;
}
