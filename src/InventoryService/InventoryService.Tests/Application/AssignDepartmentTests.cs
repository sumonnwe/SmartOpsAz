using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Inventory.AssignDepartment;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InventoryService.Tests.Application;

internal class InMemoryInventoryMovementRepository : IInventoryMovementRepository
{
    private readonly List<(Guid, int, string?, string?, string?, string?, DateTime)> _movements = new();

    public Task AddAsync(Guid inventoryId, int movementType, string? fromDepartment, string? toDepartment,
        string? fromLocation, string? toLocation, DateTime occurredAt, CancellationToken cancellationToken)
    {
        _movements.Add((inventoryId, movementType, fromDepartment, toDepartment, fromLocation, toLocation, occurredAt));
        return Task.CompletedTask;
    }

    public Task<List<(int MovementType, string? FromDepartment, string? ToDepartment, string? FromLocation, string? ToLocation, DateTime OccurredAt)>>
        GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken)
    {
        var result = _movements
            .Where(x => x.Item1 == inventoryId)
            .Select(x => (x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7))
            .ToList();
        return Task.FromResult(result);
    }
}

public class AssignDepartmentTests
{
    [Fact]
    public async Task Handle_AssignsSuccessfully()
    {
        var repo = new InMemoryInventoryRepository();
        var item = InventoryItem.Register("AT-200", "SN-200", "Device");
        await repo.AddAsync(item, CancellationToken.None);

        var movementRepo = new InMemoryInventoryMovementRepository();
        var handler = new AssignDepartmentHandler(repo, movementRepo, NullLogger<AssignDepartmentHandler>.Instance);
        var cmd = new AssignDepartmentCommand { Id = item.Id, Department = "ICU" };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.True(result);
        var fetched = await repo.GetByIdAsync(item.Id, CancellationToken.None);
        Assert.Equal("ICU", fetched?.Department);
    }

    [Fact]
    public async Task Handle_MissingItem_ReturnsFalse()
    {
        var repo = new InMemoryInventoryRepository();
        var movementRepo = new InMemoryInventoryMovementRepository();
        var handler = new AssignDepartmentHandler(repo, movementRepo, NullLogger<AssignDepartmentHandler>.Instance);
        var cmd = new AssignDepartmentCommand { Id = Guid.NewGuid(), Department = "ICU" };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_RetiredItem_Throws()
    {
        var repo = new InMemoryInventoryRepository();
        var item = InventoryItem.Register("AT-201", "SN-201", "Device");
        item.Retire();
        await repo.AddAsync(item, CancellationToken.None);

        var movementRepo = new InMemoryInventoryMovementRepository();
        var handler = new AssignDepartmentHandler(repo, movementRepo, NullLogger<AssignDepartmentHandler>.Instance);
        var cmd = new AssignDepartmentCommand { Id = item.Id, Department = "ICU" };

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(cmd, CancellationToken.None));
    }
}