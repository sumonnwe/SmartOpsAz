using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.GetInventoryById;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using Xunit;

internal class TestInventoryRepository : IInventoryRepository
{
    private readonly InventoryItem _item;

    public TestInventoryRepository(InventoryItem item)
    {
        _item = item;
    }

    public Task AddAsync(InventoryItem inventory, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<bool> ExistsByAssetTagAsync(string assetTag, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(id == _item.Id ? _item : null);

    public Task UpdateAsync(InventoryItem inventory, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<(List<InventoryItem> Items, int TotalCount)> SearchAsync(
        string? search,
        int? status,
        string? department,
        string? location,
        string? deviceType,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class GetInventoryByIdTests
{
    [Fact]
    public async Task Handle_ExistingId_ReturnsInventory()
    {
        var item = InventoryItem.Register("AT-GET-001", "SN-GET-001", "Device");
        var repo = new TestInventoryRepository(item);
        var handler = new GetInventoryByIdHandler(repo);

        var res = await handler.Handle(new GetInventoryByIdQuery(item.Id), CancellationToken.None);

        Assert.NotNull(res);
        Assert.Equal(item.Id, res.InventoryId);
        Assert.Equal(item.AssetTag, res.AssetTag);
        Assert.Equal(item.SerialNumber, res.SerialNumber);
    }

    [Fact]
    public async Task Handle_MissingId_ReturnsNull()
    {
        var item = InventoryItem.Register("AT-GET-002", "SN-GET-002", "Device");
        var repo = new TestInventoryRepository(item);
        var handler = new GetInventoryByIdHandler(repo);

        var res = await handler.Handle(new GetInventoryByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(res);
    }
}