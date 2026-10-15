using System.Threading;
using System.Threading.Tasks;
using MediatR;
using InventoryService.Application.Interfaces;
using InventoryService.Contracts.Inventory;

namespace InventoryService.Application.Inventory.GetInventoryById;

public sealed class GetInventoryByIdHandler : IRequestHandler<GetInventoryByIdQuery, GetInventoryResponse?>
{
    private readonly IInventoryRepository _repo;

    public GetInventoryByIdHandler(IInventoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<GetInventoryResponse?> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repo.GetByIdAsync(request.Id, cancellationToken);
        if (item is null) return null;

        return new GetInventoryResponse
        {
            InventoryId = item.Id,
            AssetTag = item.AssetTag,
            SerialNumber = item.SerialNumber,
            DeviceName = item.DeviceName,
            DeviceType = item.DeviceType,
            Department = item.Department,
            Location = item.Location,
            Status = (int)item.Status,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt,
            IsDeleted = item.IsDeleted
        };
    }
}