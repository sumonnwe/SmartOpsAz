using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.RegisterInventory;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InventoryService.Tests.Application;

internal class RegisterInmemoryInventoryRepository : IInventoryRepository
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

public class RegisterInventoryTests
{
    [Fact]
    public async Task Handle_NewInventory_Succeeds()
    {
        var repo = new InMemoryInventoryRepository();
        var handler = new RegisterInventoryHandler(repo, NullLogger<RegisterInventoryHandler>.Instance);

        var cmd = new RegisterInventory
        {
            AssetTag = "AT-100",
            SerialNumber = "SN-100",
            DeviceName = "Ventilator"
        };

        var res = await handler.Handle(cmd, CancellationToken.None);

        Assert.Equal("AT-100", res.AssetTag);
        Assert.Equal("SN-100", res.SerialNumber);
        Assert.NotEqual(System.Guid.Empty, res.InventoryId);
    }

    [Fact]
    public async Task Handle_DuplicateAssetTag_Throws()
    {
        var repo = new InMemoryInventoryRepository();
        // pre-add item with same asset tag
        var existing = InventoryItem.Register("AT-200", "SN-200", "Device");
        await repo.AddAsync(existing, CancellationToken.None);

        var handler = new RegisterInventoryHandler(repo, NullLogger<RegisterInventoryHandler>.Instance);

        var cmd = new RegisterInventory
        {
            AssetTag = "AT-200",
            SerialNumber = "SN-201",
            DeviceName = "Device New"
        };

        await Assert.ThrowsAsync<InventoryService.Domain.Exceptions.DomainException>(() => handler.Handle(cmd, CancellationToken.None));
    }
}