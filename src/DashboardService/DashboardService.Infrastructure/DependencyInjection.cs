using DashboardService.Infrastructure.Redis;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MassTransit;

namespace DashboardService.Infrastructure;

/// <summary>
/// Dependency injection configuration for Dashboard Infrastructure.
/// Registers Redis connection, projection store, and MassTransit consumers.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDashboardInfrastructure(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        // Configure Redis connection
        var redisConnection =
            configuration["Redis:Connection"]
            ?? configuration.GetConnectionString("Redis")
            ?? "localhost:6379";

        var options = ConfigurationOptions.Parse(redisConnection);
        options.AbortOnConnectFail = false;
        options.ConnectRetry = 3;
        options.ConnectTimeout = 5000;

        var redis = ConnectionMultiplexer.Connect(options);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        // Register projection store
        services.AddScoped<IRedisProjectionStore, RedisProjectionStore>();

        return services;
    }

    public static IServiceCollection AddDashboardConsumers(
        this IServiceCollection services)
    {
        // Consumers will be registered here in Stage 4
        return services;
    }
}