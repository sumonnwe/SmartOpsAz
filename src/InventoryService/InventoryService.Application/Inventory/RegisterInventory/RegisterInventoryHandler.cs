using System;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Inventory.RegisterInventory;

public sealed class RegisterInventoryHandler : IRequestHandler<RegisterInventory, RegisterInventoryResult>
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<RegisterInventoryHandler> _logger;

    public RegisterInventoryHandler(IInventoryRepository repository, ILogger<RegisterInventoryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<RegisterInventoryResult> Handle(RegisterInventory command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering inventory item {AssetTag}", command.AssetTag);

        // Application-level uniqueness checks (friendly error)
        if (await _repository.ExistsByAssetTagAsync(command.AssetTag, cancellationToken))
            throw new DomainException("Inventory.DuplicateAssetTag", "An inventory item with this asset tag already exists.");

        if (await _repository.ExistsBySerialNumberAsync(command.SerialNumber, cancellationToken))
            throw new DomainException("Inventory.DuplicateSerialNumber", "An inventory item with this serial number already exists.");

        // Domain factory enforces domain invariants
        var inventory = InventoryItem.Register(command.AssetTag, command.SerialNumber, command.DeviceName, command.DeviceType);

        // Persist
        await _repository.AddAsync(inventory, cancellationToken);

        return new RegisterInventoryResult
        {
            InventoryId = inventory.Id,
            AssetTag = inventory.AssetTag,
            SerialNumber = inventory.SerialNumber
        };
    }
}