using System;

namespace InventoryService.Application.Inventory.GetAssetHistory;

public sealed class AssetHistoryDto
{
    public int MovementType { get; init; }
    public string? FromDepartment { get; init; }
    public string? ToDepartment { get; init; }
    public string? FromLocation { get; init; }
    public string? ToLocation { get; init; }
    public DateTime OccurredAt { get; init; }
}