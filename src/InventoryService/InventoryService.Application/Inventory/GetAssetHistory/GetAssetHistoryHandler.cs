using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.GetAssetHistory;

public sealed class GetAssetHistoryHandler : IRequestHandler<GetAssetHistoryQuery, List<AssetHistoryDto>>
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IInventoryRepository _repository;
    private readonly ILogger<GetAssetHistoryHandler> _logger;

    public GetAssetHistoryHandler(
        IInventoryMovementRepository movementRepository,
        IInventoryRepository repository,
        ILogger<GetAssetHistoryHandler> logger)
    {
        _movementRepository = movementRepository;
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<AssetHistoryDto>> Handle(GetAssetHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving asset history for inventory {InventoryId}", request.InventoryId);

        // Verify inventory exists (including soft-deleted check)
        var item = await _repository.GetByIdAsync(request.InventoryId, cancellationToken);
        if (item is null) return new List<AssetHistoryDto>();

        var movements = await _movementRepository.GetByInventoryIdAsync(request.InventoryId, cancellationToken);

        var history = movements.Select(m => new AssetHistoryDto
        {
            MovementType = m.MovementType,
            FromDepartment = m.FromDepartment,
            ToDepartment = m.ToDepartment,
            FromLocation = m.FromLocation,
            ToLocation = m.ToLocation,
            OccurredAt = m.OccurredAt
        }).ToList();

        return history;
    }
}
