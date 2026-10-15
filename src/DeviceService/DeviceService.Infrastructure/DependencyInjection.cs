using DeviceService.Application.Interfaces;
using DeviceService.Infrastructure.HttpClients;
using DeviceService.Infrastructure.Messaging;
using DeviceService.Infrastructure.Persistence;
using DeviceService.Infrastructure.Repositories;
using DeviceService.Infrastructure.Workers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDeviceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DeviceDatabase")
            ?? configuration["DeviceDatabase:ConnectionString"]
            ?? throw new InvalidOperationException("Connection string 'DeviceDatabase' not configured.");

        services.AddDbContext<DeviceDbContext>(opts =>
        {
            opts.UseNpgsql(connStr, b => b.EnableRetryOnFailure());
        });

        services.AddScoped<IDeviceRepository, DeviceRepository>();
        //services.AddScoped<IEventPublisher, NoOpEventPublisher>();
        var rabbitEnabled = configuration.GetValue<bool>("RabbitMQ:Enabled");

        if (rabbitEnabled)
        {
            services.AddScoped<IEventPublisher, RabbitMQEventPublisher>();
        }
        else
        {
            services.AddScoped<IEventPublisher, NoOpEventPublisher>();
        }

        // HttpClient for InventoryService integration
        services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>((sp, client) =>
        {
            var inventoryUrl = configuration.GetValue<string>("InventoryService:BaseUrl")
                ?? "http://localhost:5102";
            client.BaseAddress = new Uri(inventoryUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // HttpClient for simulator to call back to DeviceService API
        services.AddHttpClient("DeviceServiceSimulator", client =>
        {
            var deviceServiceUrl = configuration.GetValue<string>("DeviceService:BaseUrl")
                ?? "http://localhost:5103";
            client.BaseAddress = new Uri(deviceServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // Background workers
        services.AddHostedService<DeviceOfflineDetectionWorker>();
        services.AddHostedService<HeartbeatSimulatorService>();

        return services;
    }
}

/// <summary>
/// Observer for RabbitMQ endpoint configuration events (used for logging).
/// </summary>
/*
internal sealed class EndpointConfigurationObserver : IEndpointConfigurationObserver
{
    public void EndpointConfigured<T>(IEndpointConfigurator configurator) where T : class
    {
        // Log or perform custom actions on endpoint configuration
    }

    void IEndpointConfigurationObserver.EndpointConfigured<T>(T configurator)
    {
        throw new NotImplementedException();
    }
}
*/