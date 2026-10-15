using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Interfaces;

public interface IInventoryMovementRepository
{
    Task AddAsync(Guid inventoryId, int movementType, string? fromDepartment, string? toDepartment,
        string? fromLocation, string? toLocation, DateTime occurredAt, CancellationToken cancellationToken);

    Task<List<(int MovementType, string? FromDepartment, string? ToDepartment, string? FromLocation, string? ToLocation, DateTime OccurredAt)>>
        GetByInventoryIdAsync(Guid inventoryId, CancellationToken cancellationToken);
}
