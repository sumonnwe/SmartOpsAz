using System;
using MediatR;

namespace InventoryService.Application.Inventory.AssignDepartment;

public sealed class AssignDepartmentCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Department { get; init; } = null!;
}
