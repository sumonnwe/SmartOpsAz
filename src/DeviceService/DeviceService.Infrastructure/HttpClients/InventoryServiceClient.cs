using DeviceService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace DeviceService.Infrastructure.HttpClients;

public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryServiceClient> _logger;

    public InventoryServiceClient(HttpClient httpClient, ILogger<InventoryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> AssetExistsAndIsActiveAsync(Guid assetId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/inventory/{assetId}", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Asset {AssetId} validated in InventoryService.", assetId);
                return true;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Asset {AssetId} not found in InventoryService.", assetId);
                return false;
            }
            _logger.LogError("InventoryService returned status {Status} for asset {AssetId}.", response.StatusCode, assetId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating asset {AssetId} with InventoryService.", assetId);
            return false;
        }
    }
}