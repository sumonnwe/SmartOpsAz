using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.DeleteInventory;

public sealed class DeleteInventoryHandler : IRequestHandler<DeleteInventoryCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<DeleteInventoryHandler> _logger;

    public DeleteInventoryHandler(IInventoryRepository repository, ILogger<DeleteInventoryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Soft-deleting inventory item {InventoryId}", request.Id);

        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return false;

        item.SoftDelete();
        await _repository.UpdateAsync(item, cancellationToken);

        return true;
    }
}
