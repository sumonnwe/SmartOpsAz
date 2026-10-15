namespace InventoryService.Contracts.Inventory;

public sealed class AssignDepartmentRequest
{
    public string Department { get; init; } = null!;
}
