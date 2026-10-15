using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.TransferLocation;

public sealed class TransferLocationHandler : IRequestHandler<TransferLocationCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly ILogger<TransferLocationHandler> _logger;

    public TransferLocationHandler(
        IInventoryRepository repository,
        IInventoryMovementRepository movementRepository,
        ILogger<TransferLocationHandler> logger)
    {
        _repository = repository;
        _movementRepository = movementRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(TransferLocationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring inventory {InventoryId} to location {Location}", request.Id, request.Location);

        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return false;

        // Domain enforces business rules (deleted, retired checks)
        item.TransferLocation(request.Location);

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
