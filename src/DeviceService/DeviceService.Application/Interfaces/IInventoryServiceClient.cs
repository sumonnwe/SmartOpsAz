namespace DeviceService.Application.Interfaces;

public interface IInventoryServiceClient
{
    Task<bool> AssetExistsAndIsActiveAsync(Guid assetId, CancellationToken cancellationToken = default);
}