using DeviceService.Application.Interfaces;
using DeviceService.Domain.Entities;
using DeviceService.Domain.Enums;
using DeviceService.Infrastructure.Persistence;
using DeviceService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeviceService.Tests.Infrastructure;

public class DeviceRepositoryTests : IAsyncLifetime
{
    private DeviceDbContext _dbContext = null!;
    private IDeviceRepository _repository = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<DeviceDbContext>()
            .UseInMemoryDatabase(databaseName: $"test-db-{Guid.NewGuid()}")
            .Options;

        _dbContext = new DeviceDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
        _repository = new DeviceRepository(_dbContext);
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_SavesDeviceToDatabase()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);

        // Act
        await _repository.AddAsync(device);
        var retrieved = await _repository.GetByIdAsync(device.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(device.Id, retrieved.Id);
        Assert.Equal("TEMP-001", retrieved.DeviceCode);
    }

    [Fact]
    public async Task GetByDeviceCodeAsync_FindsDeviceByCode()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        await _repository.AddAsync(device);

        // Act
        var retrieved = await _repository.GetByDeviceCodeAsync("TEMP-001");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(device.Id, retrieved.Id);
    }

    [Fact]
    public async Task GetByDeviceCodeAsync_ReturnsNullForNonexistent()
    {
        // Act
        var retrieved = await _repository.GetByDeviceCodeAsync("NONEXISTENT");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Original Name", DeviceType.TemperatureSensor, null, null, now);
        await _repository.AddAsync(device);
        device.UpdateName("Updated Name", now);

        // Act
        await _repository.UpdateAsync(device);
        var retrieved = await _repository.GetByIdAsync(device.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Updated Name", retrieved.Name);
    }

    [Fact]
    public async Task AddHeartbeatAsync_SavesHeartbeatRecord()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device = IoTDevice.RegisterNew("TEMP-001", "Temp Sensor", DeviceType.TemperatureSensor, null, null, now);
        await _repository.AddAsync(device);

        var hb = new DeviceHeartbeat(
            device.Id,
            now,
            85m,
            -60,
            20m,
            40m,
            "1.0.0",
            now);

        // Act
        await _repository.AddHeartbeatAsync(hb);
        var retrieved = await _repository.GetHeartbeatsAsync(device.Id);

        // Assert
        Assert.Single(retrieved);
        Assert.Equal(85m, retrieved[0].BatteryPercentage);
    }

    [Fact]
    public async Task GetDevicesWithLastHeartbeatBeforeAsync_IdentifiesOfflineDevices()
    {
        // Arrange
        var now = DateTimeOffset.UtcNow;
        var device1 = IoTDevice.RegisterNew("TEMP-001", "Temp 1", DeviceType.TemperatureSensor, null, null, now);
        var device2 = IoTDevice.RegisterNew("TEMP-002", "Temp 2", DeviceType.TemperatureSensor, null, null, now);

        await _repository.AddAsync(device1);
        await _repository.AddAsync(device2);

        // Apply heartbeat to device1 (recent)
        device1.ApplyHeartbeat(now, 85m, -60, 20m, 40m, null, now);
        await _repository.UpdateAsync(device1);

        // Apply old heartbeat to device2 (6 minutes ago)
        device2.ApplyHeartbeat(now.AddMinutes(-6), 50m, -70, 30m, 50m, null, now.AddMinutes(-6));
        await _repository.UpdateAsync(device2);

        // Threshold is 5 minutes
        var threshold = now.AddSeconds(-300);

        // Act
        var offlineDevices = await _repository.GetDevicesWithLastHeartbeatBeforeAsync(threshold);

        // Assert
        Assert.Single(offlineDevices);
        Assert.Equal(device2.Id, offlineDevices[0].Id);
    }
}