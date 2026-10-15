//csharp src/InventoryService/InventoryService.Domain/Events/InventoryMovedEvent.cs
using System;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Events;

public sealed class InventoryMovedEvent
{
    public Guid InventoryId { get; }
    public MovementType MovementType { get; }
    public string? FromDepartment { get; }
    public string? ToDepartment { get; }
    public string? FromLocation { get; }
    public string? ToLocation { get; }
    public DateTime OccurredAt { get; }

    public InventoryMovedEvent(
        Guid inventoryId,
        MovementType movementType,
        string? fromDepartment,
        string? toDepartment,
        string? fromLocation,
        string? toLocation,
        DateTime occurredAt)
    {
        InventoryId = inventoryId;
        MovementType = movementType;
        FromDepartment = fromDepartment;
        ToDepartment = toDepartment;
        FromLocation = fromLocation;
        ToLocation = toLocation;
        OccurredAt = occurredAt;
    }
}