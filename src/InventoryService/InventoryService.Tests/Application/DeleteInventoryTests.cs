using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.DeleteInventory;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InventoryService.Tests.Application;

internal class DeleteInMemoryInventoryRepository : IInventoryRepository
{
    private readonly System.Collections.Generic.List<InventoryItem> _items = new();

    public Task AddAsync(InventoryItem inventory, CancellationToken cancellationToken)
    {
        _items.Add(inventory);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(InventoryItem inventory, CancellationToken cancellationToken)
    {
        var existing = _items.FirstOrDefault(x => x.Id == inventory.Id);
        if (existing is null) return Task.CompletedTask;
        _items.Remove(existing);
        _items.Add(inventory);
        return Task.CompletedTask;
    }

    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var it = _items.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
        return Task.FromResult(it);
    }

    public Task<bool> ExistsByAssetTagAsync(string assetTag, CancellationToken cancellationToken)
    {
        return Task.FromResult(_items.Any(x => x.AssetTag == assetTag));
    }

    public Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return Task.FromResult(_items.Any(x => x.SerialNumber == serialNumber));
    }

    public Task<(List<InventoryItem> Items, int TotalCount)> SearchAsync(
        string? search, int? status, string? department, string? location, string? deviceType,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _items.Where(x => !x.IsDeleted).AsQueryable();
        var totalCount = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult((items, totalCount));
    }
}

public class DeleteInventoryTests
{
    [Fact]
    public async Task Handle_DeletesItem_Succeeds()
    {
        var repo = new InMemoryInventoryRepository();
        var item = InventoryItem.Register("AT-100", "SN-100", "Device");
        await repo.AddAsync(item, CancellationToken.None);

        var handler = new DeleteInventoryHandler(repo, NullLogger<DeleteInventoryHandler>.Instance);
        var cmd = new DeleteInventoryCommand { Id = item.Id };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result);
        var fetched = await repo.GetByIdAsync(item.Id, CancellationToken.None);
        Assert.Null(fetched); // Should not appear because IsDeleted=true and query filters
    }

    [Fact]
    public async Task Handle_MissingItem_ReturnsFalse()
    {
        var repo = new InMemoryInventoryRepository();
        var handler = new DeleteInventoryHandler(repo, NullLogger<DeleteInventoryHandler>.Instance);
        var cmd = new DeleteInventoryCommand { Id = Guid.NewGuid() };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result);
    }
}