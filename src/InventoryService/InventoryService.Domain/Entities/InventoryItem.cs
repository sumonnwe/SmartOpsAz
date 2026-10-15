using System;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Events;
using InventoryService.Domain.Exceptions;

namespace InventoryService.Domain.Entities;

public sealed class InventoryItem
{
    public Guid Id { get; private set; }
    public string AssetTag { get; private set; }
    public string SerialNumber { get; private set; }
    public string DeviceName { get; private set; }
    public string? DeviceType { get; private set; }
    public string? Department { get; private set; }
    public string? Location { get; private set; }
    public InventoryStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    private readonly List<object> _events = new();
    public IReadOnlyCollection<object> Events => _events.AsReadOnly();

    private InventoryItem() { }

    private InventoryItem(Guid id, string assetTag, string serialNumber, string deviceName, string? deviceType)
    {
        Id = id;
        AssetTag = assetTag;
        SerialNumber = serialNumber;
        DeviceName = deviceName;
        DeviceType = deviceType;
        Status = InventoryStatus.Available;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        IsDeleted = false;
    }

    public static InventoryItem Register(string assetTag, string serialNumber, string deviceName, string? deviceType = null)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new DomainException("Inventory.InvalidAssetTag", "AssetTag is required.");

        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new DomainException("Inventory.InvalidSerialNumber", "SerialNumber is required.");

        if (string.IsNullOrWhiteSpace(deviceName))
            throw new DomainException("Inventory.InvalidDeviceName", "DeviceName is required.");

        return new InventoryItem(Guid.NewGuid(), assetTag.Trim(), serialNumber.Trim(), deviceName.Trim(), deviceType?.Trim());
    }

    public void UpdateDetails(string deviceName, string? deviceType)
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot update a deleted inventory item.");
        if (string.IsNullOrWhiteSpace(deviceName)) throw new DomainException("Inventory.InvalidDeviceName", "DeviceName is required.");
        DeviceName = deviceName.Trim();
        DeviceType = deviceType?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToDepartment(string department)
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot assign a deleted inventory item.");
        if (Status == InventoryStatus.Retired) throw new DomainException("Inventory.Retired", "Retired device cannot be assigned.");

        var oldDept = Department;
        Department = department?.Trim();
        UpdatedAt = DateTime.UtcNow;

        var movementType = string.IsNullOrWhiteSpace(oldDept) ? MovementType.DepartmentAssigned : MovementType.DepartmentTransferred;
        _events.Add(new InventoryMovedEvent(Id, movementType, oldDept, Department, Location, Location, DateTime.UtcNow));
    }

    public void TransferLocation(string newLocation)
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot transfer a deleted inventory item.");
        if (Status == InventoryStatus.Retired) throw new DomainException("Inventory.Retired", "Retired device cannot be transferred.");

        var oldLocation = Location;
        Location = newLocation?.Trim();
        UpdatedAt = DateTime.UtcNow;

        _events.Add(new InventoryMovedEvent(Id, MovementType.LocationTransferred, Department, Department, oldLocation, Location, DateTime.UtcNow));
    }

    public void MarkAsMaintenance()
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot change status of a deleted inventory item.");
        if (Status == InventoryStatus.Retired) throw new DomainException("Inventory.Retired", "Retired device cannot change status.");
        Status = InventoryStatus.Maintenance;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsAvailable()
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot change status of a deleted inventory item.");
        if (Status == InventoryStatus.Retired) throw new DomainException("Inventory.Retired", "Retired device cannot change status.");
        Status = InventoryStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Retire()
    {
        if (IsDeleted) throw new DomainException("Inventory.Deleted", "Cannot retire a deleted inventory item.");
        if (Status == InventoryStatus.Retired) throw new DomainException("Inventory.AlreadyRetired", "Device is already retired.");
        Status = InventoryStatus.Retired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (IsDeleted) throw new DomainException("Inventory.AlreadyDeleted", "Device is already deleted.");
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public static InventoryItem Hydrate(Guid id, string assetTag, string serialNumber, string deviceName, string? deviceType,
        string? department, string? location, InventoryStatus status, DateTime createdAt, DateTime updatedAt, bool isDeleted)
    {
        var item = new InventoryItem();
        item.Id = id;
        item.AssetTag = assetTag;
        item.SerialNumber = serialNumber;
        item.DeviceName = deviceName;
        item.DeviceType = deviceType;
        item.Department = department;
        item.Location = location;
        item.Status = status;
        item.CreatedAt = createdAt;
        item.UpdatedAt = updatedAt;
        item.IsDeleted = isDeleted;
        return item;
    }

    public void ClearEvents() => _events.Clear();
}