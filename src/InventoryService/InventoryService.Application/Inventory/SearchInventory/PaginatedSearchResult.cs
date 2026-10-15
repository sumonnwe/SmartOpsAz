using System.Collections.Generic;
using InventoryService.Contracts.Inventory;

namespace InventoryService.Application.Inventory.SearchInventory;

public sealed class PaginatedSearchResult
{
    public List<GetInventoryResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
