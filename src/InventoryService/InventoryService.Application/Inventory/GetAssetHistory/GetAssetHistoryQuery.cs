using System;
using System.Collections.Generic;
using MediatR;

namespace InventoryService.Application.Inventory.GetAssetHistory;

public sealed class GetAssetHistoryQuery : IRequest<List<AssetHistoryDto>>
{
    public Guid InventoryId { get; init; }
}
