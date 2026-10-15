using System;

namespace InventoryService.Infrastructure.Persistence.Entities;

public sealed class InventoryMovementEntity
{
    public Guid Id { get; set; }
    public Guid InventoryId { get; set; }
    public int MovementType { get; set; }
    public string? FromDepartment { get; set; }
    public string? ToDepartment { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public DateTime OccurredAt { get; set; }
}
