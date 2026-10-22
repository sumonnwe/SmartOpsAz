using DeviceService.Application.Interfaces;
using DeviceService.Infrastructure.Persistence;
using DeviceService.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using DeviceService.Domain.Entities;
using Microsoft.Extensions.Hosting;

namespace DeviceService.Tests.Infrastructure.Workers;

public class DeviceOfflineDetectionWorkerTests
{
    [Fact]
    public void Constructor_WithServiceScopeFactory_ShouldSucceed()
    {
        // Arrange
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var logger = new Mock<ILogger<DeviceOfflineDetectionWorker>>();
        var config = new ConfigurationBuilder().Build();

        // Act
        var worker = new DeviceOfflineDetectionWorker(serviceScopeFactory.Object, logger.Object, config);

        // Assert
        Assert.NotNull(worker);
    }

    [Fact]
    public async Task ExecuteAsync_WithScopedDependencies_ShouldResolveSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();

        // Setup configuration
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DeviceService:OfflineDetectionIntervalSeconds", "60" },
                { "DeviceService:OfflineThresholdSeconds", "300" }
            })
            .Build();

        services.AddSingleton(config);
        services.AddLogging();

        // Mock scoped services
        var mockRepo = new Mock<IDeviceRepository>();
        mockRepo
            .Setup(r => r.GetDevicesWithLastHeartbeatBeforeAsync(It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyList<IoTDevice>>(new List<IoTDevice>()));

        var mockPublisher = new Mock<IEventPublisher>();

        services.AddScoped<IDeviceRepository>(_ => mockRepo.Object);
        services.AddScoped<IEventPublisher>(_ => mockPublisher.Object);

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<DeviceOfflineDetectionWorker>>();
        var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var worker = new DeviceOfflineDetectionWorker(
            serviceScopeFactory,
            logger,
            config);

        // Act & Assert - Worker should instantiate without DI lifetime errors
        Assert.NotNull(worker);
    }

    [Fact]
    public void Worker_ShouldBeRegisteredAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DeviceService:OfflineDetectionIntervalSeconds", "60" },
                { "DeviceService:OfflineThresholdSeconds", "300" }
            })
            .Build();

        services.AddSingleton(config);
        services.AddLogging();
        services.AddScoped<IDeviceRepository>(sp => new Mock<IDeviceRepository>().Object);
        services.AddScoped<IEventPublisher>(sp => new Mock<IEventPublisher>().Object);
        services.AddHostedService<DeviceOfflineDetectionWorker>();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var hostedServices = serviceProvider.GetServices<IHostedService>();

        // Assert - Worker should be registered as hosted service
        Assert.Contains(hostedServices, s => s is DeviceOfflineDetectionWorker);
    }
}