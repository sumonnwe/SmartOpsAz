//csharp src/InventoryService/InventoryService.Infrastructure/Repositories/InventoryRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Application.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public sealed class InventoryRepository : IInventoryRepository
{
    private readonly InventoryDbContext _db;

    public InventoryRepository(InventoryDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(InventoryItem inventory, CancellationToken cancellationToken)
    {
        var entity = new InventoryItemEntity
        {
            Id = inventory.Id,
            AssetTag = inventory.AssetTag,
            SerialNumber = inventory.SerialNumber,
            DeviceName = inventory.DeviceName,
            DeviceType = inventory.DeviceType,
            Department = inventory.Department,
            Location = inventory.Location,
            Status = (int)inventory.Status,
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt,
            IsDeleted = inventory.IsDeleted
        };

        _db.InventoryItems.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItem inventory, CancellationToken cancellationToken)
    {
        var entity = await _db.InventoryItems.FirstOrDefaultAsync(x => x.Id == inventory.Id, cancellationToken);
        if (entity is null)
            throw new InvalidOperationException($"Inventory item '{inventory.Id}' not found.");

        entity.DeviceName = inventory.DeviceName;
        entity.DeviceType = inventory.DeviceType;
        entity.Department = inventory.Department;
        entity.Location = inventory.Location;
        entity.Status = (int)inventory.Status;
        entity.UpdatedAt = inventory.UpdatedAt;
        entity.IsDeleted = inventory.IsDeleted;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAssetTagAsync(string assetTag, CancellationToken cancellationToken)
    {
        // Uniqueness is global, including soft-deleted items
        return await _db.InventoryItems.IgnoreQueryFilters().AnyAsync(x => x.AssetTag == assetTag, cancellationToken);
    }

    public async Task<bool> ExistsBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        // Uniqueness is global, including soft-deleted items
        return await _db.InventoryItems.IgnoreQueryFilters().AnyAsync(x => x.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var e = await _db.InventoryItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (e is null) return null;

        return InventoryItem.Hydrate(
            e.Id, e.AssetTag, e.SerialNumber, e.DeviceName, e.DeviceType,
            e.Department, e.Location, (InventoryStatus)e.Status, e.CreatedAt, e.UpdatedAt, e.IsDeleted);
    }

    public async Task<(List<InventoryItem> Items, int TotalCount)> SearchAsync(
        string? search,
        int? status,
        string? department,
        string? location,
        string? deviceType,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _db.InventoryItems.AsNoTracking().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(x =>
                x.AssetTag.ToLower().Contains(searchLower) ||
                x.SerialNumber.ToLower().Contains(searchLower) ||
                x.DeviceName.ToLower().Contains(searchLower));
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(x => x.Department == department);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(x => x.Location == location);
        }

        if (!string.IsNullOrWhiteSpace(deviceType))
        {
            query = query.Where(x => x.DeviceType == deviceType);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var skip = (page - 1) * pageSize;
        var entities = await query
            .OrderBy(x => x.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Map to domain entities
        var items = entities.Select(e => InventoryItem.Hydrate(
            e.Id, e.AssetTag, e.SerialNumber, e.DeviceName, e.DeviceType,
            e.Department, e.Location, (InventoryStatus)e.Status, e.CreatedAt, e.UpdatedAt, e.IsDeleted))
            .ToList();

        return (items, totalCount);
    }
}