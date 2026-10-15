using System.Net;
using System.Net.Http.Json;
using DeviceService.Api;
using DeviceService.Application.DTOs;
using DeviceService.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DeviceService.Tests.Api;

public class DevicesControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task RegisterDevice_WithValidRequest_Returns201()
    {
        // Arrange
        var request = new RegisterDeviceRequest(
            DeviceCode: "TEST-001",
            Name: "Test Device",
            DeviceType: DeviceType.TemperatureSensor,
            GatewayId: null,
            FirmwareVersion: "1.0.0");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/devices", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsAsync<object>();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task RegisterDevice_WithDuplicateCode_Returns409()
    {
        // Arrange
        var request = new RegisterDeviceRequest(
            DeviceCode: "DUP-001",
            Name: "Device 1",
            DeviceType: DeviceType.TemperatureSensor,
            GatewayId: null,
            FirmwareVersion: "1.0.0");

        // Act - Register first device
        await _client.PostAsJsonAsync("/api/v1/devices", request);

        // Act - Try to register duplicate
        var response2 = await _client.PostAsJsonAsync("/api/v1/devices", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    [Fact]
    public async Task SubmitHeartbeat_WithValidRequest_Returns202()
    {
        // Arrange
        var deviceRequest = new RegisterDeviceRequest(
            DeviceCode: "HB-001",
            Name: "Heartbeat Test",
            DeviceType: DeviceType.TemperatureSensor,
            GatewayId: null,
            FirmwareVersion: "1.0.0");

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/devices", deviceRequest);
        var deviceDto = await registerResponse.Content.ReadAsAsync<DeviceDto>();
        var deviceId = deviceDto!.Id;

        var hbRequest = new HeartbeatRequest(
            RecordedAtUtc: DateTimeOffset.UtcNow,
            BatteryPercentage: 85.5m,
            SignalStrengthRssi: -65,
            CpuUsagePercentage: 22.3m,
            MemoryUsagePercentage: 45.2m,
            FirmwareVersion: "1.0.1");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/devices/{deviceId}/heartbeat", hbRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task SubmitHeartbeat_WithInvalidBattery_Returns400()
    {
        // Arrange
        var deviceRequest = new RegisterDeviceRequest(
            DeviceCode: "INV-001",
            Name: "Invalid Test",
            DeviceType: DeviceType.TemperatureSensor,
            GatewayId: null,
            FirmwareVersion: "1.0.0");

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/devices", deviceRequest);
        var deviceDto = await registerResponse.Content.ReadAsAsync<DeviceDto>();
        var deviceId = deviceDto!.Id;

        var hbRequest = new HeartbeatRequest(
            RecordedAtUtc: DateTimeOffset.UtcNow,
            BatteryPercentage: 150m,  // Invalid: > 100
            SignalStrengthRssi: -65,
            CpuUsagePercentage: 22.3m,
            MemoryUsagePercentage: 45.2m,
            FirmwareVersion: "1.0.0");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/devices/{deviceId}/heartbeat", hbRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}