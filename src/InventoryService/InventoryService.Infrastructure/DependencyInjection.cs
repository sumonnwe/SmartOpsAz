//csharp src/InventoryService/InventoryService.Infrastructure/DependencyInjection.cs
using System;
using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("InventoryDatabase") ??
                   configuration["InventoryDatabase:ConnectionString"] ??
                   throw new InvalidOperationException("Connection string 'InventoryDatabase' not configured.");

        services.AddDbContext<InventoryDbContext>(opts =>
        {
            opts.UseNpgsql(conn, b => b.EnableRetryOnFailure());
        });

        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();

        return services;
    }
}