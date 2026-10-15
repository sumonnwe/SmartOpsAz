using System;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Events;
using InventoryService.Domain.Exceptions;
using Xunit;

namespace InventoryService.Tests.Domain;

public class InventoryItemTests
{
    [Fact]
    public void Register_WithValidValues_Succeeds()
    {
        var item = InventoryItem.Register("AT-001", "SN-001", "Infusion Pump", "Model X");
        Assert.NotEqual(Guid.Empty, item.Id);
        Assert.Equal("AT-001", item.AssetTag);
        Assert.Equal("SN-001", item.SerialNumber);
        Assert.Equal("Infusion Pump", item.DeviceName);
        Assert.Equal(InventoryStatus.Available, item.Status);
    }

    [Fact]
    public void Register_WithoutSerial_Throws()
    {
        Assert.Throws<DomainException>(() => InventoryItem.Register("AT-002", "", "Device"));
    }

    [Fact]
    public void Register_WithoutAssetTag_Throws()
    {
        Assert.Throws<DomainException>(() => InventoryItem.Register("", "SN-002", "Device"));
    }

    [Fact]
    public void Register_WithoutDeviceName_Throws()
    {
        Assert.Throws<DomainException>(() => InventoryItem.Register("AT-003", "SN-003", ""));
    }

    [Fact]
    public void UpdateDetails_Succeeds()
    {
        var item = InventoryItem.Register("AT-004", "SN-004", "OldName", "ModelA");
        item.UpdateDetails("NewName", "ModelB");
        Assert.Equal("NewName", item.DeviceName);
        Assert.Equal("ModelB", item.DeviceType);
    }

    [Fact]
    public void UpdateDetails_DeletedItem_Throws()
    {
        var item = InventoryItem.Register("AT-005", "SN-005", "Device");
        item.SoftDelete();
        var ex = Assert.Throws<DomainException>(() => item.UpdateDetails("NewName", null));
        Assert.Equal("Inventory.Deleted", ex.Code);
    }

    [Fact]
    public void SoftDelete_Succeeds()
    {
        var item = InventoryItem.Register("AT-006", "SN-006", "Device");
        item.SoftDelete();
        Assert.True(item.IsDeleted);
    }

    [Fact]
    public void SoftDelete_AlreadyDeleted_Throws()
    {
        var item = InventoryItem.Register("AT-007", "SN-007", "Device");
        item.SoftDelete();
        var ex = Assert.Throws<DomainException>(() => item.SoftDelete());
        Assert.Equal("Inventory.AlreadyDeleted", ex.Code);
    }

    [Fact]
    public void AssignToDepartment_Succeeds()
    {
        var item = InventoryItem.Register("AT-008", "SN-008", "Device");
        item.AssignToDepartment("ICU");
        Assert.Equal("ICU", item.Department);
        Assert.Single(item.Events);
    }

    [Fact]
    public void AssignToDepartment_DeletedItem_Throws()
    {
        var item = InventoryItem.Register("AT-009", "SN-009", "Device");
        item.SoftDelete();
        var ex = Assert.Throws<DomainException>(() => item.AssignToDepartment("ICU"));
        Assert.Equal("Inventory.Deleted", ex.Code);
    }

    [Fact]
    public void AssignToDepartment_RetiredItem_Throws()
    {
        var item = InventoryItem.Register("AT-010", "SN-010", "Device");
        item.Retire();
        var ex = Assert.Throws<DomainException>(() => item.AssignToDepartment("ICU"));
        Assert.Equal("Inventory.Retired", ex.Code);
    }

    [Fact]
    public void AssignToDepartment_GeneratesEvent()
    {
        var item = InventoryItem.Register("AT-011", "SN-011", "Device");
        item.AssignToDepartment("Cardiology");
        Assert.Single(item.Events);
        var evt = item.Events.First() as InventoryMovedEvent;
        Assert.NotNull(evt);
        Assert.Equal(MovementType.DepartmentAssigned, evt!.MovementType);
    }

    [Fact]
    public void TransferLocation_Succeeds()
    {
        var item = InventoryItem.Register("AT-012", "SN-012", "Device");
        item.TransferLocation("Room 301");
        Assert.Equal("Room 301", item.Location);
        Assert.Single(item.Events);
    }

    [Fact]
    public void TransferLocation_DeletedItem_Throws()
    {
        var item = InventoryItem.Register("AT-013", "SN-013", "Device");
        item.SoftDelete();
        var ex = Assert.Throws<DomainException>(() => item.TransferLocation("Room 301"));
        Assert.Equal("Inventory.Deleted", ex.Code);
    }

    [Fact]
    public void TransferLocation_RetiredItem_Throws()
    {
        var item = InventoryItem.Register("AT-014", "SN-014", "Device");
        item.Retire();
        var ex = Assert.Throws<DomainException>(() => item.TransferLocation("Room 301"));
        Assert.Equal("Inventory.Retired", ex.Code);
    }

    [Fact]
    public void TransferLocation_GeneratesEvent()
    {
        var item = InventoryItem.Register("AT-015", "SN-015", "Device");
        item.TransferLocation("Ward B");
        Assert.Single(item.Events);
        var evt = item.Events.First() as InventoryMovedEvent;
        Assert.NotNull(evt);
        Assert.Equal(MovementType.LocationTransferred, evt!.MovementType);
    }

    [Fact]
    public void Retire_Succeeds()
    {
        var item = InventoryItem.Register("AT-016", "SN-016", "Device");
        item.Retire();
        Assert.Equal(InventoryStatus.Retired, item.Status);
    }

    [Fact]
    public void Retire_DeletedItem_Throws()
    {
        var item = InventoryItem.Register("AT-017", "SN-017", "Device");
        item.SoftDelete();
        var ex = Assert.Throws<DomainException>(() => item.Retire());
        Assert.Equal("Inventory.Deleted", ex.Code);
    }

    [Fact]
    public void Retire_AlreadyRetired_Throws()
    {
        var item = InventoryItem.Register("AT-018", "SN-018", "Device");
        item.Retire();
        var ex = Assert.Throws<DomainException>(() => item.Retire());
        Assert.Equal("Inventory.AlreadyRetired", ex.Code);
    }
}