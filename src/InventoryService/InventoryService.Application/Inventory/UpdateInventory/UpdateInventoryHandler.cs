//csharp src/InventoryService/InventoryService.Application/Inventory/UpdateInventory/UpdateInventoryHandler.cs
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.UpdateInventory;

public sealed class UpdateInventoryHandler : IRequestHandler<UpdateInventoryCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<UpdateInventoryHandler> _logger;

    public UpdateInventoryHandler(IInventoryRepository repository, ILogger<UpdateInventoryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating inventory item {InventoryId}", request.Id);

        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return false;

        // Domain will throw DomainException for invalid state (deleted)
        item.UpdateDetails(request.DeviceName, request.DeviceType);

        await _repository.UpdateAsync(item, cancellationToken);

        return true;
    }
}