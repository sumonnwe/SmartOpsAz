using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.RetireInventory;

public sealed class RetireInventoryHandler : IRequestHandler<RetireInventoryCommand, bool>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<RetireInventoryHandler> _logger;

    public RetireInventoryHandler(IInventoryRepository repository, ILogger<RetireInventoryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(RetireInventoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retiring inventory item {InventoryId}", request.Id);

        var item = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return false;

        // Domain enforces business rules (deleted, already retired checks)
        item.Retire();

        await _repository.UpdateAsync(item, cancellationToken);

        return true;
    }
}
