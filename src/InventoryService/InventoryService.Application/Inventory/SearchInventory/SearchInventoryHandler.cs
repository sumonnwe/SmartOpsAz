using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.SearchInventory;

public sealed class SearchInventoryHandler : IRequestHandler<SearchInventoryQuery, SearchInventoryResponse>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<SearchInventoryHandler> _logger;

    public SearchInventoryHandler(IInventoryRepository repository, ILogger<SearchInventoryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SearchInventoryResponse> Handle(SearchInventoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching inventory with filters: Search={Search}, Status={Status}, Department={Department}, Location={Location}",
            request.Search, request.Status, request.Department, request.Location);

        var (items, totalCount) = await _repository.SearchAsync(
            request.Search,
            request.Status,
            request.Department,
            request.Location,
            request.DeviceType,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalPages = (totalCount + request.PageSize - 1) / request.PageSize;

        var dtos = items.Select(x => new InventoryItemDto
        {
            InventoryId = x.Id,
            AssetTag = x.AssetTag,
            SerialNumber = x.SerialNumber,
            DeviceName = x.DeviceName,
            DeviceType = x.DeviceType,
            Department = x.Department,
            Location = x.Location,
            Status = (int)x.Status,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();

        return new SearchInventoryResponse
        {
            Items = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
