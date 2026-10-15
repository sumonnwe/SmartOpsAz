using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.AssignDepartment;

public sealed class AssignDepartmentHandler : IRequestHandler<AssignDepartmentCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ILogger<AssignDepartmentHandler> _logger;

    public AssignDepartmentHandler(
        IInventoryRepository repository,
        IInventoryMovementRepository movementRepository,
        ILogger<AssignDepartmentHandler> logger)
    {
        _repository = repository;
        _movementRepository = movementRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(AssignDepartmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning inventory {InventoryId} to department {Department}", request.Id, request.Department);

        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return false;

        // Domain enforces business rules (deleted, retired checks)
        item.AssignToDepartment(request.Department);

        // Persist item and record movement history in same transaction
        await _repository.UpdateAsync(item, cancellationToken);

        // Extract and persist domain events
        foreach (var evt in item.Events)
        {
            if (evt is InventoryService.Domain.Events.InventoryMovedEvent movementEvent)
            {
                await _movementRepository.AddAsync(
                    movementEvent.InventoryId,
                    (int)movementEvent.MovementType,
                    movementEvent.FromDepartment,
                    movementEvent.ToDepartment,
                    movementEvent.FromLocation,
                    movementEvent.ToLocation,
                    movementEvent.OccurredAt,
                    cancellationToken);
            }
        }

        item.ClearEvents();

        return true;
    }
}
