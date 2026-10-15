using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Interfaces;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByAssetTagAsync(string assetTag, CancellationToken cancellationToken);
    Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
    Task AddAsync(InventoryItem inventory, CancellationToken cancellationToken);
    Task UpdateAsync(InventoryItem inventory, CancellationToken cancellationToken);
    Task<(List<InventoryItem> Items, int TotalCount)> SearchAsync(
        string? search,
        int? status,
        string? department,
        string? location,
        string? deviceType,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}