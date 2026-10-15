using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public sealed class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly InventoryDbContext _db;

    public InventoryMovementRepository(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Guid inventoryId, int movementType, string? fromDepartment, string? toDepartment, 
        string? fromLocation, string? toLocation, DateTime occurredAt, CancellationToken cancellationToken)
    {
        var entity = new InventoryMovementEntity
        {
            Id = Guid.NewGuid(),
            InventoryId = inventoryId,
            MovementType = movementType,
            FromDepartment = fromDepartment,
            ToDepartment = toDepartment,
            FromLocation = fromLocation,
            ToLocation = toLocation,
            OccurredAt = occurredAt
        };

        _db.InventoryMovements.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<(int MovementType, string? FromDepartment, string? ToDepartment, string? FromLocation, string? ToLocation, DateTime OccurredAt)>> 
        GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken)
    {
        return await _db.InventoryMovements
            .AsNoTracking()
            .Where(x => x.InventoryId == inventoryId)
            .OrderBy(x => x.OccurredAt)
            .Select(x => new ValueTuple<int, string?, string?, string?, string?, DateTime>(
                x.MovementType, x.FromDepartment, x.ToDepartment, x.FromLocation, x.ToLocation, x.OccurredAt))
            .ToListAsync(cancellationToken);
    }
}
