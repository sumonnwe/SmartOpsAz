using MediatR;

namespace InventoryService.Application.Inventory.SearchInventory;

public sealed class SearchInventoryQuery : IRequest<SearchInventoryResponse>
{
    public string? Search { get; init; }
    public int? Status { get; init; }
    public string? Department { get; init; }
    public string? Location { get; init; }
    public string? DeviceType { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
