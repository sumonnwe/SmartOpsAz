using DeviceService.Domain.Entities;
using DeviceService.Domain.Enums;
using DeviceService.Domain.Events;
using DeviceService.Domain.Exceptions;
using Xunit;

namespace DeviceService.Tests.Domain;

public class IoTDeviceTests
{
    [Fact]
    public void RegisterNew_ValidInput_CreatesDeviceInOfflineStatus()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Act
        var device = IoTDevice.RegisterNew(
            "TEMP-ICU-001",
            "ICU Temperature Sensor",
            DeviceType.TemperatureSensor,
            null,
            "1.0.0",
            now);

        // Assert
        Assert.NotEqual(Guid.Empty, device.Id);
        Assert.Equal("TEMP-ICU-001", device.DeviceCode);
        Assert.Equal("ICU Temperature Sensor", device.Name);
        Assert.Equal(DeviceType.TemperatureSensor, device.DeviceType);
        Assert.Equal(DeviceStatus.Offline, device.Status);
        Assert.Null(device.LastHeartbeatAtUtc);
        Assert.Single(device.Events);
        Assert.IsType<DeviceRegisteredEvent>(device.Events.First());
    }

    [Fact]
    public void RegisterNew_InvalidName_ThrowsException()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            IoTDevice.RegisterNew("TEMP-001", "A", DeviceType.TemperatureSensor, null, null, now));
        Assert.Equal("Device.InvalidName", ex.Code);
    }

    [Fact]
    public void ApplyHeartbeat_ValidData_UpdatesStatusToOnline()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.ClearEvents();

        // Act
        device.ApplyHeartbeat(
            recordedAtUtc: now,
            battery: 85.5m,
            rssi: -65,
            cpu: 22.3m,
            memory: 45.2m,
            firmware: "1.0.1",
            serverReceivedAtUtc: now);

        // Assert
        Assert.Equal(DeviceStatus.Online, device.Status);
        Assert.Equal(85.5m, device.BatteryPercentage);
        Assert.Equal(-65, device.SignalStrengthRssi);
        Assert.Equal("1.0.1", device.FirmwareVersion);
        Assert.Equal(now, device.LastHeartbeatAtUtc);
    }

    [Fact]
    public void ApplyHeartbeat_FromOffline_PublishesRecoveryEvent()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.ClearEvents();
        Assert.Equal(DeviceStatus.Offline, device.Status);

        // Act
        device.ApplyHeartbeat(now, 85m, -60, 20m, 40m, null, now);

        // Assert
        Assert.Equal(DeviceStatus.Online, device.Status);
        Assert.Single(device.Events);
        Assert.IsType<DeviceRecoveredEvent>(device.Events.First());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ApplyHeartbeat_InvalidBattery_ThrowsException(decimal battery)
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            device.ApplyHeartbeat(now, battery, -60, 20m, 40m, null, now));
        Assert.Equal("Device.InvalidBattery", ex.Code);
    }

    [Theory]
    [InlineData(-121)]
    [InlineData(1)]
    public void ApplyHeartbeat_InvalidRSSI_ThrowsException(int rssi)
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            device.ApplyHeartbeat(now, 85m, rssi, 20m, 40m, null, now));
        Assert.Equal("Device.InvalidRSSI", ex.Code);
    }

    [Fact]
    public void MarkOffline_FromOnline_PublishesOfflineEvent()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.ClearEvents();
        device.ApplyHeartbeat(now, 85m, -60, 20m, 40m, null, now);
        device.ClearEvents();
        Assert.Equal(DeviceStatus.Online, device.Status);

        // Act
        device.MarkOffline(now.AddSeconds(301));

        // Assert
        Assert.Equal(DeviceStatus.Offline, device.Status);
        Assert.Single(device.Events);
        Assert.IsType<DeviceOfflineEvent>(device.Events.First());
        Assert.True(device.HasPublishedOfflineEvent);
    }

    [Fact]
    public void MarkOffline_WhenAlreadyPublished_DoesNotPublishDuplicateEvent()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.ClearEvents();
        device.ApplyHeartbeat(now, 85m, -60, 20m, 40m, null, now);
        device.MarkOffline(now.AddSeconds(301));
        device.ClearEvents();

        // Act
        device.MarkOffline(now.AddSeconds(600));

        // Assert
        Assert.Empty(device.Events);
        Assert.Equal(DeviceStatus.Offline, device.Status);
    }

    [Fact]
    public void SetMaintenance_ChangesStatusToMaintenance()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);

        // Act
        device.SetMaintenance(now);

        // Assert
        Assert.Equal(DeviceStatus.Maintenance, device.Status);
    }

    [Fact]
    public void ApplyHeartbeat_WhenInMaintenance_DoesNotChangeStatus()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.SetMaintenance(now);
        device.ClearEvents();

        // Act
        device.ApplyHeartbeat(now, 85m, -60, 20m, 40m, null, now);

        // Assert
        Assert.Equal(DeviceStatus.Maintenance, device.Status);
    }

    [Fact]
    public void Activate_FromMaintenance_ChangesStatusToOnline()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        device.SetMaintenance(now);

        // Act
        device.Activate(now);

        // Assert
        Assert.Equal(DeviceStatus.Online, device.Status);
    }

    [Fact]
    public void LinkAsset_UpdatesLinkedAssetId()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        var assetId = Guid.NewGuid();

        // Act
        device.LinkAsset(assetId, now);

        // Assert
        Assert.Equal(assetId, device.LinkedAssetId);
    }
}